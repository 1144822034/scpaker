using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NVorbis
{
	internal abstract class VorbisFloor
	{
		internal abstract class PacketData
		{
			internal int BlockSize;

			public abstract bool HasEnergy
			{
				get;
			}

			internal bool ForceEnergy
			{
				get;
				set;
			}

			internal bool ForceNoEnergy
			{
				get;
				set;
			}

			internal bool ExecuteChannel => (ForceEnergy | HasEnergy) & !ForceNoEnergy;
		}

		public class Floor0 : VorbisFloor
		{
			public class PacketData0 : PacketData
			{
				internal float[] Coeff;

				internal float Amp;

				public override bool HasEnergy => Amp > 0f;
			}

			public int _order;

			public int _rate;

			public int _bark_map_size;

			public int _ampBits;

			public int _ampOfs;

			public int _ampDiv;

			public VorbisCodebook[] _books;

			public int _bookBits;

			public Dictionary<int, float[]> _wMap;

			public Dictionary<int, int[]> _barkMaps;

			public PacketData0[] _reusablePacketData;

			internal Floor0(VorbisStreamDecoder vorbis)
				: base(vorbis)
			{
			}

			public override void Init(DataPacket packet)
			{
				_order = (int)packet.ReadBits(8);
				_rate = (int)packet.ReadBits(16);
				_bark_map_size = (int)packet.ReadBits(16);
				_ampBits = (int)packet.ReadBits(6);
				_ampOfs = (int)packet.ReadBits(8);
				_books = new VorbisCodebook[(int)packet.ReadBits(4) + 1];
				if (_order < 1 || _rate < 1 || _bark_map_size < 1 || _books.Length == 0)
				{
					throw new InvalidDataException();
				}
				_ampDiv = (1 << _ampBits) - 1;
				for (int i = 0; i < _books.Length; i++)
				{
					int num = (int)packet.ReadBits(8);
					if (num < 0 || num >= _vorbis.Books.Length)
					{
						throw new InvalidDataException();
					}
					VorbisCodebook vorbisCodebook = _vorbis.Books[num];
					if (vorbisCodebook.MapType == 0 || vorbisCodebook.Dimensions < 1)
					{
						throw new InvalidDataException();
					}
					_books[i] = vorbisCodebook;
				}
				_bookBits = Utils.ilog(_books.Length);
				_barkMaps = new Dictionary<int, int[]>();
				_barkMaps[_vorbis.Block0Size] = SynthesizeBarkCurve(_vorbis.Block0Size / 2);
				_barkMaps[_vorbis.Block1Size] = SynthesizeBarkCurve(_vorbis.Block1Size / 2);
				_wMap = new Dictionary<int, float[]>();
				_wMap[_vorbis.Block0Size] = SynthesizeWDelMap(_vorbis.Block0Size / 2);
				_wMap[_vorbis.Block1Size] = SynthesizeWDelMap(_vorbis.Block1Size / 2);
				_reusablePacketData = new PacketData0[_vorbis._channels];
				for (int j = 0; j < _reusablePacketData.Length; j++)
				{
					_reusablePacketData[j] = new PacketData0
					{
						Coeff = new float[_order + 1]
					};
				}
			}

			public int[] SynthesizeBarkCurve(int n)
			{
				float num = (float)_bark_map_size / toBARK(_rate / 2);
				int[] array = new int[n + 1];
				for (int i = 0; i < n - 1; i++)
				{
					array[i] = Math.Min(_bark_map_size - 1, (int)Math.Floor(toBARK((float)_rate / 2f / (float)n * (float)i) * num));
				}
				array[n] = -1;
				return array;
			}

			public static float toBARK(double lsp)
			{
				return (float)(13.1 * Math.Atan(0.00074 * lsp) + 2.24 * Math.Atan(1.85E-08 * lsp * lsp) + 0.0001 * lsp);
			}

			public float[] SynthesizeWDelMap(int n)
			{
				float num = (float)(Math.PI / (double)_bark_map_size);
				float[] array = new float[n];
				for (int i = 0; i < n; i++)
				{
					array[i] = 2f * (float)Math.Cos(num * (float)i);
				}
				return array;
			}

			internal override PacketData UnpackPacket(DataPacket packet, int blockSize, int channel)
			{
				PacketData0 packetData = _reusablePacketData[channel];
				packetData.BlockSize = blockSize;
				packetData.ForceEnergy = false;
				packetData.ForceNoEnergy = false;
				packetData.Amp = (float)(double)packet.ReadBits(_ampBits);
				if (packetData.Amp > 0f)
				{
					Array.Clear(packetData.Coeff, 0, packetData.Coeff.Length);
					packetData.Amp = packetData.Amp / (float)_ampDiv * (float)_ampOfs;
					uint num = (uint)packet.ReadBits(_bookBits);
					if (num >= _books.Length)
					{
						packetData.Amp = 0f;
						return packetData;
					}
					VorbisCodebook vorbisCodebook = _books[num];
					int i = 0;
					while (i < _order)
					{
						int num2 = vorbisCodebook.DecodeScalar(packet);
						if (num2 == -1)
						{
							packetData.Amp = 0f;
							return packetData;
						}
						int num3 = 0;
						for (; i < _order; i++)
						{
							if (num3 >= vorbisCodebook.Dimensions)
							{
								break;
							}
							packetData.Coeff[i] = vorbisCodebook[num2, num3];
							num3++;
						}
					}
					float num4 = 0f;
					int num5 = 0;
					while (num5 < _order)
					{
						int num6 = 0;
						while (num5 < _order && num6 < vorbisCodebook.Dimensions)
						{
							packetData.Coeff[num5] += num4;
							num5++;
							num6++;
						}
						num4 = packetData.Coeff[num5 - 1];
					}
				}
				return packetData;
			}

			internal override void Apply(PacketData packetData, float[] residue)
			{
				PacketData0 packetData2 = packetData as PacketData0;
				if (packetData2 == null)
				{
					throw new ArgumentException("Incorrect packet data!");
				}
				int num = packetData2.BlockSize / 2;
				if (packetData2.Amp > 0f)
				{
					int[] array = _barkMaps[packetData2.BlockSize];
					float[] array2 = _wMap[packetData2.BlockSize];
					int num2 = 0;
					for (num2 = 0; num2 < _order; num2++)
					{
						packetData2.Coeff[num2] = 2f * (float)Math.Cos(packetData2.Coeff[num2]);
					}
					num2 = 0;
					while (num2 < num)
					{
						int num3 = array[num2];
						float num4 = 0.5f;
						float num5 = 0.5f;
						float num6 = array2[num3];
						int i;
						for (i = 1; i < _order; i += 2)
						{
							num5 *= num6 - packetData2.Coeff[i - 1];
							num4 *= num6 - packetData2.Coeff[i];
						}
						if (i == _order)
						{
							num5 *= num6 - packetData2.Coeff[i - 1];
							num4 *= num4 * (4f - num6 * num6);
							num5 *= num5;
						}
						else
						{
							num4 *= num4 * (2f - num6);
							num5 *= num5 * (2f + num6);
						}
						num5 = packetData2.Amp / (float)Math.Sqrt(num4 + num5) - (float)_ampOfs;
						num5 = (float)Math.Exp(num5 * 0.115129247f);
						residue[num2] *= num5;
						while (array[++num2] == num3)
						{
							residue[num2] *= num5;
						}
					}
				}
				else
				{
					Array.Clear(residue, 0, num);
				}
			}
		}

		public class Floor1 : VorbisFloor
		{
			public class PacketData1 : PacketData
			{
				public int[] Posts = new int[64];

				public int PostCount;

				public override bool HasEnergy => PostCount > 0;
			}

			public int[] _partitionClass;

			public int[] _classDimensions;

			public int[] _classSubclasses;

			public int[] _xList;

			public int[] _classMasterBookIndex;

			public int[] _hNeigh;

			public int[] _lNeigh;

			public int[] _sortIdx;

			public int _multiplier;

			public int _range;

			public int _yBits;

			public VorbisCodebook[] _classMasterbooks;

			public VorbisCodebook[][] _subclassBooks;

			public int[][] _subclassBookIndex;

			public static int[] _rangeLookup = new int[4]
			{
				256,
				128,
				86,
				64
			};

			public static int[] _yBitsLookup = new int[4]
			{
				8,
				7,
				7,
				6
			};

			public PacketData1[] _reusablePacketData;

			public bool[] _stepFlags = new bool[64];

			public int[] _finalY = new int[64];

			public static readonly float[] inverse_dB_table = new float[256]
			{
				1.06498632E-07f,
				1.1341951E-07f,
				1.20790148E-07f,
				1.28639783E-07f,
				1.369995E-07f,
				1.459025E-07f,
				1.55384086E-07f,
				1.65481808E-07f,
				1.76235744E-07f,
				1.87688556E-07f,
				1.998856E-07f,
				2.128753E-07f,
				2.26709133E-07f,
				2.41441967E-07f,
				2.57132228E-07f,
				2.73842119E-07f,
				2.91637917E-07f,
				3.10590224E-07f,
				3.307741E-07f,
				3.52269666E-07f,
				3.75162131E-07f,
				3.995423E-07f,
				4.255068E-07f,
				4.53158634E-07f,
				4.82607447E-07f,
				5.1397E-07f,
				5.47370632E-07f,
				5.829419E-07f,
				6.208247E-07f,
				6.611694E-07f,
				7.041359E-07f,
				7.49894639E-07f,
				7.98627E-07f,
				8.505263E-07f,
				9.057983E-07f,
				9.646621E-07f,
				1.02735135E-06f,
				1.0941144E-06f,
				1.16521608E-06f,
				1.24093845E-06f,
				1.32158164E-06f,
				1.40746545E-06f,
				1.49893049E-06f,
				1.59633942E-06f,
				1.70007854E-06f,
				1.81055918E-06f,
				1.92821949E-06f,
				2.053526E-06f,
				2.18697573E-06f,
				2.3290977E-06f,
				2.48045581E-06f,
				2.64164964E-06f,
				2.813319E-06f,
				2.9961443E-06f,
				3.19085052E-06f,
				3.39821E-06f,
				3.619045E-06f,
				3.85423073E-06f,
				4.10470057E-06f,
				4.371447E-06f,
				4.6555283E-06f,
				4.958071E-06f,
				5.280274E-06f,
				5.623416E-06f,
				5.988857E-06f,
				6.37804669E-06f,
				6.79252844E-06f,
				7.23394533E-06f,
				7.704048E-06f,
				8.2047E-06f,
				8.737888E-06f,
				9.305725E-06f,
				9.910464E-06f,
				1.05545014E-05f,
				1.12403923E-05f,
				1.19708557E-05f,
				1.27487892E-05f,
				1.3577278E-05f,
				1.44596061E-05f,
				1.53992714E-05f,
				1.64000048E-05f,
				1.74657689E-05f,
				1.86007928E-05f,
				1.98095768E-05f,
				2.10969138E-05f,
				2.24679115E-05f,
				2.39280016E-05f,
				2.54829774E-05f,
				2.71390054E-05f,
				2.890265E-05f,
				3.078091E-05f,
				3.27812268E-05f,
				3.49115326E-05f,
				3.718028E-05f,
				3.95964671E-05f,
				4.21696677E-05f,
				4.491009E-05f,
				4.7828602E-05f,
				5.09367746E-05f,
				5.424693E-05f,
				5.77722021E-05f,
				6.152657E-05f,
				6.552491E-05f,
				6.97830837E-05f,
				7.43179844E-05f,
				7.914758E-05f,
				8.429104E-05f,
				8.976875E-05f,
				9.560242E-05f,
				0.000101815211f,
				0.000108431741f,
				0.000115478237f,
				0.000122982674f,
				0.000130974775f,
				0.000139486248f,
				0.000148550855f,
				0.000158204537f,
				0.000168485552f,
				0.00017943469f,
				0.000191095358f,
				0.000203513817f,
				0.0002167393f,
				0.000230824226f,
				0.000245824485f,
				0.000261799549f,
				0.000278812746f,
				0.000296931568f,
				0.000316227874f,
				0.000336778146f,
				0.000358663878f,
				0.000381971884f,
				0.00040679457f,
				0.000433230365f,
				0.0004613841f,
				0.0004913675f,
				0.00052329927f,
				0.0005573062f,
				0.0005935231f,
				0.0006320936f,
				0.0006731706f,
				0.000716917f,
				0.0007635063f,
				0.000813123246f,
				0.000865964568f,
				0.000922239851f,
				0.0009821722f,
				0.00104599923f,
				0.00111397426f,
				0.00118636654f,
				0.00126346329f,
				0.0013455702f,
				0.00143301289f,
				0.00152613816f,
				0.00162531529f,
				0.00173093739f,
				0.00184342347f,
				0.00196321961f,
				0.00209080055f,
				0.0022266726f,
				0.00237137428f,
				0.00252547953f,
				0.00268959929f,
				0.00286438479f,
				0.0030505287f,
				0.003248769f,
				0.00345989247f,
				0.00368473586f,
				0.00392419053f,
				0.00417920668f,
				0.004450795f,
				0.004740033f,
				0.005048067f,
				0.0053761187f,
				0.005725489f,
				0.00609756354f,
				0.00649381755f,
				0.00691582263f,
				0.00736525143f,
				0.007843887f,
				0.008353627f,
				0.008896492f,
				0.009474637f,
				0.010090352f,
				0.01074608f,
				0.0114444206f,
				0.012188144f,
				0.0129801976f,
				0.0138237253f,
				0.0147220679f,
				0.0156787913f,
				0.0166976862f,
				0.0177827962f,
				0.0189384222f,
				0.0201691482f,
				0.0214798544f,
				0.0228757355f,
				0.02436233f,
				0.0259455312f,
				0.0276316181f,
				0.0294272769f,
				0.0313396268f,
				0.03337625f,
				0.0355452262f,
				0.0378551558f,
				0.0403152f,
				0.0429351069f,
				0.0457252748f,
				0.0486967564f,
				0.05186135f,
				0.05523159f,
				0.05882085f,
				0.0626433641f,
				0.06671428f,
				0.07104975f,
				0.0756669641f,
				0.08058423f,
				0.08582105f,
				0.09139818f,
				0.0973377451f,
				0.1036633f,
				0.110399932f,
				0.117574342f,
				0.125214979f,
				0.133352146f,
				0.142018124f,
				0.151247263f,
				0.161076173f,
				0.1715438f,
				0.182691678f,
				0.194564015f,
				0.207207873f,
				0.220673427f,
				0.235014021f,
				0.250286549f,
				0.266551584f,
				0.283873618f,
				0.3023213f,
				0.32196787f,
				0.342891127f,
				0.365174145f,
				0.3889052f,
				0.414178461f,
				0.44109413f,
				0.4697589f,
				0.50028646f,
				0.532797933f,
				0.5674221f,
				0.6042964f,
				0.643566966f,
				0.6853896f,
				0.729930043f,
				0.777365f,
				0.8278826f,
				(float)Math.PI * 277f / 987f,
				0.9389798f,
				1f
			};

			internal Floor1(VorbisStreamDecoder vorbis)
				: base(vorbis)
			{
			}

			public override void Init(DataPacket packet)
			{
				_partitionClass = new int[(uint)packet.ReadBits(5)];
				for (int i = 0; i < _partitionClass.Length; i++)
				{
					_partitionClass[i] = (int)packet.ReadBits(4);
				}
				int num = _partitionClass.Max();
				_classDimensions = new int[num + 1];
				_classSubclasses = new int[num + 1];
				_classMasterbooks = new VorbisCodebook[num + 1];
				_classMasterBookIndex = new int[num + 1];
				_subclassBooks = new VorbisCodebook[num + 1][];
				_subclassBookIndex = new int[num + 1][];
				for (int j = 0; j <= num; j++)
				{
					_classDimensions[j] = (int)packet.ReadBits(3) + 1;
					_classSubclasses[j] = (int)packet.ReadBits(2);
					if (_classSubclasses[j] > 0)
					{
						_classMasterBookIndex[j] = (int)packet.ReadBits(8);
						_classMasterbooks[j] = _vorbis.Books[_classMasterBookIndex[j]];
					}
					_subclassBooks[j] = new VorbisCodebook[1 << _classSubclasses[j]];
					_subclassBookIndex[j] = new int[_subclassBooks[j].Length];
					for (int k = 0; k < _subclassBooks[j].Length; k++)
					{
						int num2 = (int)packet.ReadBits(8) - 1;
						if (num2 >= 0)
						{
							_subclassBooks[j][k] = _vorbis.Books[num2];
						}
						_subclassBookIndex[j][k] = num2;
					}
				}
				_multiplier = (int)packet.ReadBits(2);
				_range = _rangeLookup[_multiplier];
				_yBits = _yBitsLookup[_multiplier];
				_multiplier++;
				int num3 = (int)packet.ReadBits(4);
				List<int> list = new List<int>();
				list.Add(0);
				list.Add(1 << num3);
				for (int l = 0; l < _partitionClass.Length; l++)
				{
					int num4 = _partitionClass[l];
					for (int m = 0; m < _classDimensions[num4]; m++)
					{
						list.Add((int)packet.ReadBits(num3));
					}
				}
				_xList = list.ToArray();
				_lNeigh = new int[list.Count];
				_hNeigh = new int[list.Count];
				_sortIdx = new int[list.Count];
				_sortIdx[0] = 0;
				_sortIdx[1] = 1;
				for (int n = 2; n < _lNeigh.Length; n++)
				{
					_lNeigh[n] = 0;
					_hNeigh[n] = 1;
					_sortIdx[n] = n;
					for (int num5 = 2; num5 < n; num5++)
					{
						int num6 = _xList[num5];
						if (num6 < _xList[n])
						{
							if (num6 > _xList[_lNeigh[n]])
							{
								_lNeigh[n] = num5;
							}
						}
						else if (num6 < _xList[_hNeigh[n]])
						{
							_hNeigh[n] = num5;
						}
					}
				}
				for (int num7 = 0; num7 < _sortIdx.Length - 1; num7++)
				{
					for (int num8 = num7 + 1; num8 < _sortIdx.Length; num8++)
					{
						if (_xList[num7] == _xList[num8])
						{
							throw new InvalidDataException();
						}
						if (_xList[_sortIdx[num7]] > _xList[_sortIdx[num8]])
						{
							int num9 = _sortIdx[num7];
							_sortIdx[num7] = _sortIdx[num8];
							_sortIdx[num8] = num9;
						}
					}
				}
				_reusablePacketData = new PacketData1[_vorbis._channels];
				for (int num10 = 0; num10 < _reusablePacketData.Length; num10++)
				{
					_reusablePacketData[num10] = new PacketData1();
				}
			}

			internal override PacketData UnpackPacket(DataPacket packet, int blockSize, int channel)
			{
				PacketData1 packetData = _reusablePacketData[channel];
				packetData.BlockSize = blockSize;
				packetData.ForceEnergy = false;
				packetData.ForceNoEnergy = false;
				packetData.PostCount = 0;
				Array.Clear(packetData.Posts, 0, 64);
				if (packet.ReadBit())
				{
					int num = 2;
					packetData.Posts[0] = (int)packet.ReadBits(_yBits);
					packetData.Posts[1] = (int)packet.ReadBits(_yBits);
					for (int i = 0; i < _partitionClass.Length; i++)
					{
						int num2 = _partitionClass[i];
						int num3 = _classDimensions[num2];
						int num4 = _classSubclasses[num2];
						int num5 = (1 << num4) - 1;
						uint num6 = 0u;
						if (num4 > 0 && (num6 = (uint)_classMasterbooks[num2].DecodeScalar(packet)) == uint.MaxValue)
						{
							num = 0;
							break;
						}
						for (int j = 0; j < num3; j++)
						{
							VorbisCodebook vorbisCodebook = _subclassBooks[num2][num6 & num5];
							num6 >>= num4;
							if (vorbisCodebook != null && (packetData.Posts[num] = vorbisCodebook.DecodeScalar(packet)) == -1)
							{
								num = 0;
								i = _partitionClass.Length;
								break;
							}
							num++;
						}
					}
					packetData.PostCount = num;
				}
				return packetData;
			}

			internal override void Apply(PacketData packetData, float[] residue)
			{
				PacketData1 packetData2 = packetData as PacketData1;
				if (packetData2 == null)
				{
					throw new ArgumentException("Incorrect packet data!", "packetData");
				}
				int num = packetData2.BlockSize / 2;
				if (packetData2.PostCount > 0)
				{
					bool[] array = UnwrapPosts(packetData2);
					int num2 = 0;
					int num3 = packetData2.Posts[0] * _multiplier;
					for (int i = 1; i < packetData2.PostCount; i++)
					{
						int num4 = _sortIdx[i];
						if (array[num4])
						{
							int num5 = _xList[num4];
							int num6 = packetData2.Posts[num4] * _multiplier;
							if (num2 < num)
							{
								RenderLineMulti(num2, num3, Math.Min(num5, num), num6, residue);
							}
							num2 = num5;
							num3 = num6;
						}
						if (num2 >= num)
						{
							break;
						}
					}
					if (num2 < num)
					{
						RenderLineMulti(num2, num3, num, num3, residue);
					}
				}
				else
				{
					Array.Clear(residue, 0, num);
				}
			}

			public bool[] UnwrapPosts(PacketData1 data)
			{
				Array.Clear(_stepFlags, 2, 62);
				_stepFlags[0] = true;
				_stepFlags[1] = true;
				Array.Clear(_finalY, 2, 62);
				_finalY[0] = data.Posts[0];
				_finalY[1] = data.Posts[1];
				for (int i = 2; i < data.PostCount; i++)
				{
					int num = _lNeigh[i];
					int num2 = _hNeigh[i];
					int num3 = RenderPoint(_xList[num], _finalY[num], _xList[num2], _finalY[num2], _xList[i]);
					int num4 = data.Posts[i];
					int num5 = _range - num3;
					int num6 = num3;
					int num7 = (num5 >= num6) ? (num6 * 2) : (num5 * 2);
					if (num4 != 0)
					{
						_stepFlags[num] = true;
						_stepFlags[num2] = true;
						_stepFlags[i] = true;
						if (num4 >= num7)
						{
							if (num5 > num6)
							{
								_finalY[i] = num4 - num6 + num3;
							}
							else
							{
								_finalY[i] = num3 - num4 + num5 - 1;
							}
						}
						else if (num4 % 2 == 1)
						{
							_finalY[i] = num3 - (num4 + 1) / 2;
						}
						else
						{
							_finalY[i] = num3 + num4 / 2;
						}
					}
					else
					{
						_stepFlags[i] = false;
						_finalY[i] = num3;
					}
				}
				for (int j = 0; j < data.PostCount; j++)
				{
					data.Posts[j] = _finalY[j];
				}
				return _stepFlags;
			}

			public int RenderPoint(int x0, int y0, int x1, int y1, int X)
			{
				int num = y1 - y0;
				int num2 = x1 - x0;
				int num3 = Math.Abs(num) * (X - x0) / num2;
				if (num < 0)
				{
					return y0 - num3;
				}
				return y0 + num3;
			}

			public void RenderLineMulti(int x0, int y0, int x1, int y1, float[] v)
			{
				int num = y1 - y0;
				int num2 = x1 - x0;
				int num3 = Math.Abs(num);
				int num4 = 1 - ((num >> 31) & 1) * 2;
				int num5 = num / num2;
				int num6 = x0;
				int num7 = y0;
				int num8 = -num2;
				v[x0] *= inverse_dB_table[y0];
				num3 -= Math.Abs(num5) * num2;
				while (++num6 < x1)
				{
					num7 += num5;
					num8 += num3;
					if (num8 >= 0)
					{
						num8 -= num2;
						num7 += num4;
					}
					v[num6] *= inverse_dB_table[num7];
				}
			}
		}

		public VorbisStreamDecoder _vorbis;

		internal static VorbisFloor Init(VorbisStreamDecoder vorbis, DataPacket packet)
		{
			int num = (int)packet.ReadBits(16);
			VorbisFloor vorbisFloor = null;
			switch (num)
			{
			case 0:
				vorbisFloor = new Floor0(vorbis);
				break;
			case 1:
				vorbisFloor = new Floor1(vorbis);
				break;
			}
			if (vorbisFloor == null)
			{
				throw new InvalidDataException();
			}
			vorbisFloor.Init(packet);
			return vorbisFloor;
		}

		public VorbisFloor(VorbisStreamDecoder vorbis)
		{
			_vorbis = vorbis;
		}

		public abstract void Init(DataPacket packet);

		internal abstract PacketData UnpackPacket(DataPacket packet, int blockSize, int channel);

		internal abstract void Apply(PacketData packetData, float[] residue);
	}
}
