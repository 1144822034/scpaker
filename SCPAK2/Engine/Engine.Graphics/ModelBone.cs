using System.Collections.Generic;

namespace Engine.Graphics
{
	public class ModelBone
	{
		internal List<ModelBone> m_childBones = new List<ModelBone>();

		internal Matrix m_transform;

		public Model Model
		{
			get;
			internal set;
		}

		public int Index
		{
			get;
			internal set;
		}

		public string Name
		{
			get;
			set;
		}

		public Matrix Transform
		{
			get
			{
				return m_transform;
			}
			set
			{
				m_transform = value;
			}
		}

		public ModelBone ParentBone
		{
			get;
			internal set;
		}

		public ReadOnlyList<ModelBone> ChildBones => new ReadOnlyList<ModelBone>(m_childBones);

		internal ModelBone()
		{
		}
	}
}
