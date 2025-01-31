////////////////////////////////////////
//                                    //
//   Generated by CEO's YAAAG - V1.2  //
// (Yet Another Arya Addon Generator) //
//                                    //
////////////////////////////////////////

namespace Server.Items
{
    public class hex_WaterfallPondAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {13579, 0, 1, 3}, {1822, 0, 0, 3}, {1823, 0, 1, 1}// 1	2	3	
			, {13456, 0, 0, 8}, {1822, 0, 0, 1}, {13597, 0, 2, 1}// 4	5	6	
			, {13501, 1, 1, 1}, {13501, 1, 2, 1}, {13501, 1, 0, 1}// 7	8	9	
			, {231, 1, 0, 1}, {231, 1, 1, 1}, {231, -1, 2, 1}// 10	11	12	
			, {231, -1, 1, 1}, {220, 1, 2, 0}, {222, 0, 2, 1}// 13	14	15	
			, {222, 1, -1, 1}, {222, 0, -1, 1}, {222, 0, -1, 4}// 16	17	18	
			, {222, 0, -1, 7}, {221, -1, 0, 4}, {221, -1, 1, 4}// 19	20	21	
			, {221, -1, 0, 7}, {224, 1, -1, 4}, {227, -1, 1, 7}// 22	23	24	
			, {227, -1, 2, 4}, {223, -1, -1, 7}, {3219, 1, 0, 7}// 25	26	27	
			, {3204, 0, 0, 10}, {3248, 1, 1, 3}, {3334, 0, 1, 14}// 28	29	30	
			, {7952, 1, 0, 2}, {15872, 2, 1, 1}, {15872, 2, 2, 1}// 31	32	33	
			, {4552, 2, 0, 3}// 34	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new hex_WaterfallPondAddonDeed();
			}
		}

		[ Constructable ]
		public hex_WaterfallPondAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public hex_WaterfallPondAddon( Serial serial ) : base( serial )
		{
		}


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class hex_WaterfallPondAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new hex_WaterfallPondAddon();
			}
		}

		[Constructable]
		public hex_WaterfallPondAddonDeed()
		{
			Name = "hex_WaterfallPond";
		}

		public hex_WaterfallPondAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}