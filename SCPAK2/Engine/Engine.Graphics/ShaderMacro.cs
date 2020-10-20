using System;

namespace Engine.Graphics
{
	public class ShaderMacro
	{
		public static string m_nameChars1 = "_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

		public static string m_nameChars2 = "_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

		public readonly string Name;

		public readonly string Value;

		public ShaderMacro(string name)
			: this(name, string.Empty)
		{
		}

		public ShaderMacro(string name, string value)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			for (int i = 0; i < name.Length; i++)
			{
				if ((i == 0 && m_nameChars1.IndexOf(name[i]) == -1) || (i > 0 && m_nameChars2.IndexOf(name[i]) == -1))
				{
					throw new ArgumentException("Invalid shader macro name.");
				}
			}
			if (value.IndexOf('\n') != -1 || (value.Length > 0 && (char.IsWhiteSpace(value[0]) || char.IsWhiteSpace(value[value.Length - 1]))))
			{
				throw new ArgumentException("Invalid shader macro value.");
			}
			Name = name;
			Value = value;
		}
	}
}
