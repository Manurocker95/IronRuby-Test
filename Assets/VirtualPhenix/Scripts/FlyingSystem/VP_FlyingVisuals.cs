using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Controllers.Components;

namespace VirtualPhenix.FlyingSystem
{
    public class VP_FlyingVisuals : VP_CharacterComponent
    {
        /*
        [Header("Visual")]
        [SerializeField] protected Transform m_movementMesh;
        
        [SerializeField] protected Transform m_leftWing;
        [SerializeField] protected Transform m_rightWing;
        [SerializeField] protected Transform m_hipsPos;

        protected float m_timeBtwFallingEffects;

        [Header("VisualFx")]
        [SerializeField] protected VP_VFXDefaultDatabase m_visualFX;

        [Header("Flying Fx")]
        protected float m_timeBtwFlyFx;

        [Header("WindFx")]
        [SerializeField] protected GameObject m_windFx;
        [SerializeField] protected AudioSource m_windAudio;
        [SerializeField] protected float m_windAudioMax;
        [SerializeField] protected float m_windLerpAmt;
        [SerializeField] protected float m_windLerpSpeed;

        protected override void Start()
        {
            base.Start();

            m_timeBtwFallingEffects = 1.8f;
        }

        public virtual void Step()
        {
            Vector3 pos = transform.position;
            GameObject obj = VP_VFXPoolManager.Instance.InstantiateVFX(VP_VFXSetup.FlyingSystem.FLOOR_STEP);
        }

        public void Jump()
        {
            VP_VFXPoolManager.Instance.InstantiateVFX(VP_VFXSetup.FlyingSystem.JUMP);
            Instantiate(JumpFx, transform.position, MovementMesh.transform.rotation);

            if (JumpAudio)
                Instantiate(JumpAudio, transform.position, Quaternion.identity).GetComponent<PlayAudio>();
        }
        public void Landing()
        {
            if (LandingFx)
            {
                Instantiate(LandingFx, transform.position, MovementMesh.transform.rotation);

            }
        }

        //effect checks
        public void SetFallingEffects(float Amt)
        {
            TimeBtwFallingEffects = Amt;
        }

        public void FallEffectCheck(float D)
        {
            if (!WindFx)
                return;

            if (TimeBtwFallingEffects > 0)
            {
                TimeBtwFallingEffects -= D;
                return;
            }

            GameObject Wind = Instantiate(WindFx, transform.position, transform.rotation);
            Wind.transform.parent = this.transform;

            TimeBtwFallingEffects = Random.Range(0.6f, 1.2f);

        }

        public void FlyingFxTimer(float D)
        {
            if (TimeBtwFlyFx > 0)
                TimeBtwFlyFx -= D;
            else
            {
                TimeBtwFlyFx = 0.5f;

                //wing trails
                if (WingTrail)
                {
                    if (LeftWing)
                    {
                        GameObject LeftWingFX = Instantiate(WingTrail, m_leftWing.transform.position, Quaternion.identity);
                        LeftWingFX.transform.parent = LeftWing;
                    }
                    if (m_rightWing)
                    {
                        GameObject RightWingFx = Instantiate(m_wingTrail, m_rightWing.transform.position, Quaternion.identity);
                        RightWingFx.transform.parent = m_rightWing;
                    }
                }
            }
        }

        public void WindAudioSetting(float D, float VelocityMagnitude)
        {
            if (!WindAudio)
                return;

            float LerpAmt = VelocityMagnitude / 40;

            WindLerpAmt = Mathf.Lerp(WindLerpAmt, LerpAmt, D * WindLerpSpeed);

            WindAudio.volume = Mathf.Lerp(0, WindAudioMax, WindLerpAmt);
        }

        */
    }
}