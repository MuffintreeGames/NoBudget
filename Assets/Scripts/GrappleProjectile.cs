using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{

	public struct StartGrappleEvent
	{
		public GrappleProjectile GrappleHook;

		public StartGrappleEvent(GrappleProjectile grappleHook)
		{
			GrappleHook = grappleHook;
		}

		static StartGrappleEvent e;
		public static void Trigger(GrappleProjectile grappleHook)
		{
			e.GrappleHook = grappleHook;
			MMEventManager.TriggerEvent(e);
		}
	}

	public class GrappleProjectile : Projectile
	{
		/// the layers that can be grappled onto
		[Tooltip("the layers that can be grappled onto")]
		public LayerMask GrappleableLayerMask;

		public virtual void OnTriggerEnter2D(Collider2D collider)
		{
			StartGrapple(collider);
		}

		protected virtual void StartGrapple(Collider2D collider)
		{
			if (!this.isActiveAndEnabled)
			{
				return;
			}

			// if what we're colliding with isn't part of the target layers, we do nothing and exit
			if (!MMLayers.LayerInLayerMask(collider.gameObject.layer, GrappleableLayerMask))
			{
				return;
			}

			Speed = 0;
			CancelInvoke();
			Debug.Log("time to grapple!");
			StartGrappleEvent.Trigger(this);
			/*_colliderHealth = collider.gameObject.MMGetComponentNoAlloc<Health>();

			OnHit?.Invoke();

			// if what we're colliding with is damageable
			if ((_colliderHealth != null) && (_colliderHealth.enabled))
			{
				if (_colliderHealth.CurrentHealth > 0)
				{
					OnCollideWithDamageable(_colliderHealth);
				}
			}
			// if what we're colliding with can't be damaged
			else
			{
				OnCollideWithNonDamageable();
			}*/
		}
	}
}