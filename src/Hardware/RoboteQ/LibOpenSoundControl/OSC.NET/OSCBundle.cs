using System;
using System.Collections;

namespace OSC.NET
{
	/// <summary>
	/// OSCBundle ÇÃäTóvÇÃê‡ñæÇ≈Ç∑ÅB
	/// </summary>
	public class OSCBundle : OSCPacket
	{
		protected const string BUNDLE = "#bundle";

		public OSCBundle()
		{
			this.address = BUNDLE;
		}

		override protected void pack()
		{
			ArrayList data = new ArrayList();

			addBytes(data, packString(this.Address));
			padNull(data);
			addBytes(data, packLong(0)); // TODO
			
			foreach(object value in this.Values)
			{
				if(value is OSCPacket)
				{
					byte[] bs = ((OSCPacket)value).BinaryData;
					addBytes(data, packInt(bs.Length));
					addBytes(data, bs);
				}
				else 
				{
					// TODO
				}
			}
			
			this.binaryData = (byte[])data.ToArray(typeof(byte));
		}

		public static new OSCBundle Unpack(byte[] bytes, ref int start, int end)
		{
			OSCBundle bundle = new OSCBundle();

			string address = unpackString(bytes, ref start);
			if(!address.Equals(BUNDLE)) return null; // TODO

			long time = unpackLong(bytes, ref start);
			while(start < end)
			{
				int subEnd = unpackInt(bytes, ref start);
				bundle.Append(OSCPacket.Unpack(bytes, ref start, subEnd));

			}


			return bundle;
		}

		override public void Append(object value)
		{
			if( value is OSCPacket) 
			{
				values.Add(value);
			}
			else 
			{
				// TODO: exception
			}
		}

		override public bool IsBundle() { return true; }
	}
}
