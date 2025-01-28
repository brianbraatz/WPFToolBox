using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using GadgetFramework;
using System.Reflection;

namespace XPGadgets
{
    public class KnownGadgets
    {
        public static IList<Type> Gadgets = new List<Type>();

        static KnownGadgets()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".\\Gadgets"));
            Assembly assembly;

			for (int index = 0; index < 3; ++index)
			{
				foreach (FileInfo fileInfo in directoryInfo.GetFiles())
				{
					try
					{
						assembly = Assembly.LoadFile(fileInfo.FullName);
					}
					catch (Exception)
					{
						continue;
					}

					foreach (Type possibleType in assembly.GetTypes())
					{
						if (typeof(Gadget).IsAssignableFrom(possibleType))
						{
							Gadgets.Add(possibleType);
						}
					}
				}
			}
        }
    }
}
