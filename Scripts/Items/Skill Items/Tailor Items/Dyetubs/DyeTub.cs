using System.Collections.Generic;
using Server.Multis;
using Server.Targeting;
using Server.ContextMenus;
using Server.Gumps;

namespace Server.Items
{
    public interface IDyable
	{
		bool Dye( Mobile from, DyeTub sender );
	}

	public class DyeTub : Item, ISecurable
	{
		private bool m_Redyable;
		private int m_DyedHue;
		private SecureLevel m_SecureLevel;

		public virtual CustomHuePicker CustomHuePicker{ get{ return null; } }

		public virtual bool AllowRunebooks
		{
			get{ return false; }
		}

		public virtual bool AllowFurniture
		{
			get{ return false; }
		}

		public virtual bool AllowStatuettes
		{
			get{ return false; }
		}

		public virtual bool AllowLeather
		{
			get{ return false; }
		}

		public virtual bool AllowDyables
		{
			get{ return true; }
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
			
			writer.Write( (int)m_SecureLevel );
			writer.Write( (bool) m_Redyable );
			writer.Write( (int) m_DyedHue );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_SecureLevel = (SecureLevel)reader.ReadInt();
					goto case 0;
				}
				case 0:
				{
					m_Redyable = reader.ReadBool();
					m_DyedHue = reader.ReadInt();

					break;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual bool Redyable
		{
			get
			{
				return m_Redyable;
			}
			set
			{
				m_Redyable = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int DyedHue
		{
			get
			{
				return m_DyedHue;
			}
			set
			{
				if ( m_Redyable )
				{
					m_DyedHue = value;
					Hue = value;
				}
			}
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public SecureLevel Level
		{
			get
			{
				return m_SecureLevel;
			}
			set
			{
				m_SecureLevel = value;
			}
		}

		[Constructable] 
		public DyeTub() : base( 0xFAB )
		{
			Weight = 10.0;
			m_Redyable = true;
		}
		
		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
			SetSecureLevelEntry.AddTo( from, this, list );
		}

		public DyeTub( Serial serial ) : base( serial )
		{
		}

		// Three metallic tubs now.
		public virtual bool MetallicHues{ get { return false; } }

		// Select the clothing to dye.
		public virtual int TargetMessage{ get{ return 500859; } }

		// You can not dye that.
		public virtual int FailMessage{ get{ return 1042083; } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( this.GetWorldLocation(), 1 ) )
			{
				from.SendLocalizedMessage( TargetMessage );
				from.Target = new InternalTarget( this );
			}
			else
			{
				from.SendLocalizedMessage( 500446 ); // That is too far away.
			}
		}

		private class InternalTarget : Target
		{
			private DyeTub m_Tub;

			public InternalTarget( DyeTub tub ) : base( 1, false, TargetFlags.None )
			{
				m_Tub = tub;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Item )
				{
					Item item = (Item)targeted;

					if ( item.QuestItem )
					{
						from.SendLocalizedMessage( 1151836 ); // You may not dye toggled quest items.
					}
					else if ( item is IDyable && m_Tub.AllowDyables )
					{
						if ( !from.InRange( m_Tub.GetWorldLocation(), 1 ) || !from.InRange( item.GetWorldLocation(), 1 ) )
							from.SendLocalizedMessage( 500446 ); // That is too far away.
						else if ( item.Parent is Mobile )
							from.SendLocalizedMessage( 500861 ); // Can't Dye clothing that is being worn.
						else if ( ((IDyable)item).Dye( from, m_Tub ) )
							from.PlaySound( 0x23E );
					}
					else if ( (FurnitureAttribute.Check( item ) || (item is PotionKeg)) && m_Tub.AllowFurniture )
					{
						if ( !from.InRange( m_Tub.GetWorldLocation(), 1 ) || !from.InRange( item.GetWorldLocation(), 1 ) )
						{
							from.SendLocalizedMessage( 500446 ); // That is too far away.
						}
						else
						{
							bool okay = ( item.IsChildOf( from.Backpack ) );

							if ( !okay )
							{
								if ( item.Parent == null )
								{
									BaseHouse house = BaseHouse.FindHouseAt( item );

									if ( house == null || ( !house.IsLockedDown( item ) && !house.IsSecure( item ) ) )
										from.SendLocalizedMessage( 501022 ); // Furniture must be locked down to paint it.
									else if ( !house.IsCoOwner( from ) )
										from.SendLocalizedMessage( 501023 ); // You must be the owner to use this item.
									else
										okay = true;
								}
								else
								{
									from.SendLocalizedMessage( 1048135 ); // The furniture must be in your backpack to be painted.
								}
							}

							if ( okay )
							{
								item.Hue = m_Tub.DyedHue;
								from.PlaySound( 0x23E );
							}
						}
					}
					else if ( (item is Runebook || item is RecallRune ) && m_Tub.AllowRunebooks )
					{
						if ( !from.InRange( m_Tub.GetWorldLocation(), 1 ) || !from.InRange( item.GetWorldLocation(), 1 ) )
						{
							from.SendLocalizedMessage( 500446 ); // That is too far away.
						}
						else if ( !item.Movable )
						{
							from.SendLocalizedMessage( 1049776 ); // You cannot dye runes or runebooks that are locked down.
						}
						else
						{
							item.Hue = m_Tub.DyedHue;
							from.PlaySound( 0x23E );
						}
					}
					else if ( item is MonsterStatuette && m_Tub.AllowStatuettes )
					{
						if ( !from.InRange( m_Tub.GetWorldLocation(), 1 ) || !from.InRange( item.GetWorldLocation(), 1 ) )
						{
							from.SendLocalizedMessage( 500446 ); // That is too far away.
						}
						else if ( !item.Movable )
						{
							from.SendLocalizedMessage( 1049779 ); // You cannot dye statuettes that are locked down.
						}
						else
						{
							item.Hue = m_Tub.DyedHue;
							from.PlaySound( 0x23E );
						}
					}
					else if ( (item is BaseArmor && (((BaseArmor)item).MaterialType == ArmorMaterialType.Leather || ((BaseArmor)item).MaterialType == ArmorMaterialType.Studded) || item is ElvenBoots || item is WoodlandBelt) && m_Tub.AllowLeather )
					{
						if ( !from.InRange( m_Tub.GetWorldLocation(), 1 ) || !from.InRange( item.GetWorldLocation(), 1 ) )
						{
							from.SendLocalizedMessage( 500446 ); // That is too far away.
						}
						else if ( !item.Movable )
						{
							from.SendLocalizedMessage( 1042419 ); // You may not dye leather items which are locked down.
						}
						else if ( item.Parent is Mobile )
						{
							from.SendLocalizedMessage( 500861 ); // Can't Dye clothing that is being worn.
						}
						else
						{
							item.Hue = m_Tub.DyedHue;
							from.PlaySound( 0x23E );
						}
					}
					else
					{
						from.SendLocalizedMessage( m_Tub.FailMessage );
					}
				}
				else
				{
					from.SendLocalizedMessage( m_Tub.FailMessage );
				}
			}
		}
	}
}