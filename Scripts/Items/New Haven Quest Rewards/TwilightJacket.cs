namespace Server.Items
{
    public class TwilightJacket : LeatherNinjaJacket
	{
		public override int LabelNumber{ get{ return 1078183; } } // Twilight Jacket

		public override int BasePhysicalResistance{ get{ return 6; } }
		public override int BaseFireResistance{ get{ return 12; } }
		public override int BaseColdResistance{ get{ return 3; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 3; } }

		[Constructable]
		public TwilightJacket()
		{
			LootType = LootType.Blessed;

			Attributes.ReflectPhysical = 5;
		}

		public TwilightJacket( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}
