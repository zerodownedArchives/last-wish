using System;
using Server.Items;
using Server.ContextMenus;
using System.Collections.Generic;
using System.Collections;

namespace Server.Mobiles
{
    public class BaseHire : BaseCreature
	{
		private int m_Pay = 1;
		private bool m_IsHired;
		private int m_HoldGold = 8;
		private Timer m_PayTimer;

		public BaseHire( AIType AI ): base( AI, FightMode.Aggressor, 10, 1, 0.1, 4.0 )
		{
		}

		public BaseHire(): base( AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.1, 4.0 )
		{
		}

		public BaseHire( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );// version

			writer.Write( (bool)m_IsHired );
			writer.Write( (int)m_HoldGold );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_IsHired = reader.ReadBool();
			m_HoldGold = reader.ReadInt();

			m_PayTimer = new PayTimer( this );
			m_PayTimer.Start();
		}

		private static Hashtable m_HireTable = new Hashtable();

		public static Hashtable HireTable 
		{ 
			get{ return m_HireTable;}
		}

		public override bool KeepsItemsOnDeath{ get{ return true;}}
		
		private int m_GoldOnDeath = 0;
		
		public override bool OnBeforeDeath()
		{ 
			// Stop the pay timer if its running 
			if( m_PayTimer != null ) 
				m_PayTimer.Stop();

			m_PayTimer = null;

			// Get all of the gold on the hireling and add up the total amount 
			if( this.Backpack != null )
			{
				Item[] AllGold = this.Backpack.FindItemsByType( typeof(Gold), true );
				if( AllGold != null )
				{
					foreach( Gold g in AllGold )
						this.m_GoldOnDeath += g.Amount;
				}
			}

			return base.OnBeforeDeath();
		}

		public override void OnDeath( Container c )
		{ 
			if( this.m_GoldOnDeath > 0 ) 
				c.DropItem( new Gold( this.m_GoldOnDeath ) );

			base.OnDeath( c );
		}

		[CommandProperty( AccessLevel.Player )]
		public bool IsHired
		{
			get
			{
				return m_IsHired;
			}
			set
			{
				if ( m_IsHired== value )
					return;

				m_IsHired= value;
				Delta( MobileDelta.Noto );
				InvalidateProperties();
			}
		}

		#region [ GetOwner ]
		public virtual Mobile GetOwner()
		{
			if( !Controlled )
				return null;
			Mobile Owner = ControlMaster;
	  
			m_IsHired = true;
	  
			if( Owner == null )
				return null;
	  
			if( Owner.Deleted || Owner.Map != this.Map || !Owner.InRange( Location, 30 ) )
			{
				Say( 1005653 );// Hmmm.  I seem to have lost my master.
				BaseHire.HireTable.Remove( Owner );
				SetControlMaster( null );
				return null;
			}

			return Owner;
		}
		#endregion 

		#region [ AddHire ] 
		public virtual bool AddHire( Mobile m ) 
		{ 
			Mobile owner = GetOwner();

			if( owner != null ) 
			{ 
				m.SendLocalizedMessage( 1043283, owner.Name );// I am following ~1_NAME~. 
				return false;
			}

			if( SetControlMaster( m ) ) 
			{ 
				m_IsHired = true;
				return true;
			}
	  
			return false;
		}
		#endregion 

		#region [ Payday ] 
		public virtual bool Payday( BaseHire m ) 
		{ 
			m_Pay = (int)m.Skills[SkillName.Anatomy].Value + (int)m.Skills[SkillName.Tactics].Value;
			m_Pay += (int)m.Skills[SkillName.Macing].Value + (int)m.Skills[SkillName.Swords].Value;
			m_Pay += (int)m.Skills[SkillName.Fencing].Value + (int)m.Skills[SkillName.Archery].Value;
			m_Pay += (int)m.Skills[SkillName.MagicResist].Value + (int)m.Skills[SkillName.Healing].Value;
			m_Pay += (int)m.Skills[SkillName.Magery].Value + (int)m.Skills[SkillName.Parry].Value;
			m_Pay /= 35;
			m_Pay += 1;
			return true;
		}
		#endregion 

		#region [ OnDragDrop ]
		public override bool OnDragDrop( Mobile from, Item item )
		{ 
			if( m_Pay != 0 )
			{ 
				// Is the creature already hired
				if( Controlled == false )
				{ 
					// Is the item the payment in gold
					if( item is Gold )
					{ 
						// Is the payment in gold sufficient
						if( item.Amount >= m_Pay )
						{ 
							// Check if this mobile already has a hire
							BaseHire hire = (BaseHire)m_HireTable[from];

							if( hire != null && !hire.Deleted && hire.GetOwner() == from )
							{
								SayTo( from, 500896 );// I see you already have an escort.
								return false;
							}

							// Try to add the hireling as a follower
							if( AddHire(from) == true )
							{ 
								SayTo( from, 1043258, string.Format( "{0}", (int)item.Amount / m_Pay ) );//"I thank thee for paying me. I will work for thee for ~1_NUMBER~ days.", (int)item.Amount / m_Pay );
								m_HireTable[from] = this;
								m_HoldGold += item.Amount;
								m_PayTimer = new PayTimer( this );
								m_PayTimer.Start();
								return true;
							}

							else
							{
								return false;
							}
						}

						else 
						{ 
							this.SayHireCost();
						}

					}

					else 
					{
						SayTo( from, 1043268 );// Tis crass of me, but I want gold
					}
				}

				else 
				{ 
					Say( 1042495 );// I have already been hired.
				}
			}

			else 
			{
				SayTo( from, 500200 );// I have no need for that.
			}

			return base.OnDragDrop( from, item );
		}
		#endregion 


		#region [ OnSpeech ] 
		internal void SayHireCost() 
		{ 
			Say( 1043256, string.Format( "{0}", m_Pay ) );// "I am available for hire for ~1_AMOUNT~ gold coins a day. If thou dost give me gold, I will work for thee."
		}

		public override void OnSpeech( SpeechEventArgs e ) 
		{	
			if( !e.Handled && e.Mobile.InRange( this, 6 ) ) 
			{ 
				int[] keywords = e.Keywords;
				string speech = e.Speech;

				// Check for a greeting, a 'hire', or a 'servant'
				if( ( e.HasKeyword( 0x003B ) == true ) || ( e.HasKeyword( 0x0162 ) == true ) || ( e.HasKeyword( 0x000C ) == true ) )
				{
					if( Controlled == false )
					{
						e.Handled = Payday( this );
						this.SayHireCost();
					}
					else
					{
						this.Say( 1042495 );// I have already been hired.
					}
				}
			}

			base.OnSpeech( e );
		}
		#endregion	

		#region [ GetContextMenuEntries ] 
		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			BaseAI m_AI = new MeleeAI(this);
		
			if ( this.Deleted )
				return;

			if ( CanPaperdollBeOpenedBy( from ) )
				list.Add( new PaperdollEntry( this ) );

			if ( from == this && Backpack != null && CanSee( Backpack ) && CheckAlive( false ) )
				list.Add( new OpenBackpackEntry( this ) );

			if( Controlled == false )
				list.Add( new HireEntry( from, this )  );

			if( Controlled == true ) // OSI has a custom MenuEntries for Hireling, with Dismiss instead of Release (see BaseAI changes)
				m_AI.GetContextMenuEntries( from, list );
		}
		
		#endregion 
	
		#region [ Class PayTimer ] 
		private class PayTimer : Timer 
		{ 
			private BaseHire m_Hire;
	  
			public PayTimer( BaseHire vend ) : base( TimeSpan.FromMinutes( 30.0 ), TimeSpan.FromMinutes( 30.0 ) ) 
			{ 
				m_Hire = vend;
				Priority = TimerPriority.OneMinute;
			}
	  
			protected override void OnTick() 
			{ 
				int m_Pay = m_Hire.m_Pay;
				if( m_Hire.m_HoldGold <= m_Pay ) 
				{ 
					// Get the current owner, if any (updates HireTable) 
					Mobile owner = m_Hire.GetOwner();

					m_Hire.Say( 503235 );// I regret nothing!postal 
					m_Hire.Delete();
				}

				else 
				{ 
					m_Hire.m_HoldGold -= m_Pay;
				}
			}
		}
		#endregion 

		#region [ Class HireEntry ]
		public class HireEntry : ContextMenuEntry
		{ 
			private Mobile m_Mobile;
			private BaseHire m_Hire;

			public HireEntry( Mobile from, BaseHire hire ) : base( 6120, 3 )
			{ 
				m_Hire = hire;
				m_Mobile = from;
			}
	  
			public override void OnClick()
			{
				m_Hire.Payday(m_Hire);
				m_Hire.SayHireCost();
			}
		}
		#endregion
	}
}