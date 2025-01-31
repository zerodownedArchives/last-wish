namespace Server.Items
{
    public class BladeDance : RuneBlade
	{
		public override int LabelNumber{ get{ return 1075033; } } // Blade Dance

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public BladeDance()
		{
			Hue = 0x66C;

			Attributes.BonusMana = 8;
			Attributes.SpellChanneling = 1;
			Attributes.WeaponDamage = 30;
			WeaponAttributes.HitLeechMana = 20;
			WeaponAttributes.UseBestSkill = 1;
		}

		public BladeDance( Serial serial ) : base( serial )
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