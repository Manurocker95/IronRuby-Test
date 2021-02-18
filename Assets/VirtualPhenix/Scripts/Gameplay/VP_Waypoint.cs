using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
	public class VP_Waypoint : VP_MonoBehaviour
    {
	    public bool m_selected = false;
	    [SerializeField] protected float m_radius = 0.2f;
        [SerializeField] protected Color m_colorInGizmo = Color.blue;
	    [SerializeField] protected Color m_colorSelectedInGizmo = Color.green;
	    [SerializeField] protected Transform m_target;

	    protected override void Initialize()
	    {
	    	base.Initialize();
	    	
	    	if (!m_target)
		    	m_target = transform;
	    }

	    protected virtual void OnDrawGizmos()
        {
	        Gizmos.color = m_selected ? m_colorSelectedInGizmo : m_colorInGizmo;
	        Gizmos.DrawSphere(m_target != null ? m_target.position : transform.position, m_radius);
        }
    }

}
