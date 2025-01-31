namespace Server.Items
{
    public class BookOfChivalry : Spellbook
	{
		public override SpellbookType SpellbookType{ get{ return SpellbookType.Paladin; } }
		public override int BookOffset{ get{ return 200; } }
		public override int BookCount{ get{ return 10; } }

		[Constructable]
		public BookOfChivalry() : this( (ulong)0x3FF )
		{
		}

		[Constructable]
		public BookOfChivalry( ulong content ) : base( content, 0x2252 )
		{
			Layer = (Core.ML ? Layer.OneHanded : Layer.Invalid);
		}

		public BookOfChivalry( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if( version == 0 && Core.ML )
				Layer = Layer.OneHanded;
		}
	}
}