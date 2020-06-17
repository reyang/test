// CshNet.cmd str2guid.cs <str>

using System;
using System.Security.Cryptography;

public class Program
{
	public static Guid GenerateGuidFromName(string name)
	{
		// This method is case-insensitive
		name = name.ToUpperInvariant();

		// The algorithm below is following the guidance of http://www.ietf.org/rfc/rfc4122.txt
		// Create a blob containing a 16 byte number representing the namespace
		// followed by the unicode bytes in the name.  
		var bytes = new byte[name.Length * 2 + 16];
		uint namespace1 = 0X482C2DB2;
		uint namespace2 = 0XC39047C8;
		uint namespace3 = 0X87F81A15;
		uint namespace4 = 0XBFC130FB;

		// Write the bytes most-significant byte first.  
		for (int i = 3; 0 <= i; --i)
		{
			bytes[i] = (byte)namespace1;
			namespace1 >>= 8;
			bytes[i + 4] = (byte)namespace2;
			namespace2 >>= 8;
			bytes[i + 8] = (byte)namespace3;
			namespace3 >>= 8;
			bytes[i + 12] = (byte)namespace4;
			namespace4 >>= 8;
		}

		// Write out  the name, most significant byte first
		for (int i = 0; i < name.Length; i++)
		{
			bytes[2 * i + 16 + 1] = (byte)name[i];
			bytes[2 * i + 16] = (byte)(name[i] >> 8);
		}

		// Compute the SHA-1 hash
		var sha1 = SHA1.Create();
		byte[] hash = sha1.ComputeHash(bytes);

		// Create a GUID out of the first 16 bytes of the hash (SHA-1 create a 20 byte hash)
		int a = (((((hash[3] << 8) + hash[2]) << 8) + hash[1]) << 8) + hash[0];
		short b = (short)((hash[5] << 8) + hash[4]);
		short c = (short)((hash[7] << 8) + hash[6]);

		c = (short)((c & 0x0FFF) | 0x5000);   // Set high 4 bits of octet 7 to 5, as per RFC 4122
		Guid guid = new Guid(a, b, c, hash[8], hash[9], hash[10], hash[11], hash[12], hash[13], hash[14], hash[15]);
		return guid;
	}

	static int Main(string[] args)
	{
		if (args.Length == 0)
		{
			System.Console.WriteLine("{0} <str>", System.AppDomain.CurrentDomain.FriendlyName);
			return -1;
		}
		System.Console.WriteLine(GenerateGuidFromName(args[0]));
		return 0;
	}
}
