using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 // Script made with Animator to FSM Conversor
namespace VirtualPhenix
{
      public class VP_FSM_Example : MonoBehaviour
      {

          // States in every layer 
          public enum STATES
          {
              A = 0,
              B = 1,
              C = 2,
          }

          // First state, first layer
          [SerializeField] private STATES m_currentState = STATES.A;

          private delegate void TransitionDetectionDelegate();
          private TransitionDetectionDelegate m_TD_State;

          private delegate void UpdateStateDelegate();
          private UpdateStateDelegate m_US_State;

         // Animator Params ap_...
          [SerializeField] Animator m_animator = null;
          [SerializeField] bool m_PassAnimatorParamsToAnimator=true;
          [SerializeField] float ap_NewFloat = 0f;
          [SerializeField] int ap_NewInt = 0;
          [SerializeField] bool ap_NewBool = false;
          [SerializeField] bool ap_NewTrigger = false;

          void Awake()
          {

          }

          void Start()
          {

             m_currentState = STATES.A;
             m_TD_State = TransitionDetection_A;
             m_US_State = UpdateStateA; 

          }

          void Update()
          {

              m_TD_State();
              m_US_State();
              m_UP();

          }



          void PassAnimatorParamsToAnimator()
          {
              m_animator.SetFloat("NewFloat", ap_NewFloat); 
              m_animator.SetInteger("NewInt", ap_NewInt); 
              m_animator.SetBool("NewBool", ap_NewBool); 
              if(ap_NewTrigger){
                 m_animator.SetTrigger("NewTrigger");
               ap_NewTrigger=false;
            }
          }

          ///Pass the param values from the FSM->AnimController
          void m_UP()
          {
               if(m_PassAnimatorParamsToAnimator && m_animator!=null ){
                   PassAnimatorParamsToAnimator();
               }
          }


          void UpdateStateA()
          {
                // TODO: UPDATE THIS STATE
                if (m_currentState == STATES.A)
                {

                }
          }


          bool TD_FromAToB()
          {
              // TODO CHECK IF A GOES TO B
                return 
                 ap_NewFloat<5 && 
                 ap_NewFloat>0;
          }



          void TransitionFromAToB()
          {
              m_currentState = STATES.B;
              m_TD_State = TransitionDetection_B;
              m_US_State = UpdateStateB;
              // TODO ANY OTHER CHANGES WHEN GOING FROM A TO B
          }




          bool TD_FromAToC()
          {
              // TODO CHECK IF A GOES TO C
                return 
                 ap_NewInt==4;
          }



          void TransitionFromAToC()
          {
              m_currentState = STATES.C;
              m_TD_State = TransitionDetection_C;
              m_US_State = UpdateStateC;
              // TODO ANY OTHER CHANGES WHEN GOING FROM A TO C
          }



          void UpdateStateB()
          {
              // TODO: UPDATE THIS STATE
          }


          bool TD_FromBToC()
          {
              // TODO CHECK IF B GOES TO C
                return 
                 ap_NewBool;
          }



          void TransitionFromBToC()
          {
              m_currentState = STATES.C;
              m_TD_State = TransitionDetection_C;
              m_US_State = UpdateStateC;
              // TODO ANY OTHER CHANGES WHEN GOING FROM B TO C
          }



          void UpdateStateC()
          {
              // TODO: UPDATE THIS STATE
          }


          bool TD_FromCToA()
          {
              // TODO CHECK IF C GOES TO A
                return 
                 ap_NewTrigger;
          }



          void TransitionFromCToA()
          {
              m_currentState = STATES.A;
              m_TD_State = TransitionDetection_A;
              m_US_State = UpdateStateA;
              // TODO ANY OTHER CHANGES WHEN GOING FROM C TO A
          }




          void TransitionDetection_A()
          {
              // CHECK IF A GOES TO B
              if(TD_FromAToB())
                  {TransitionFromAToB();}
              // CHECK IF A GOES TO C
              else if(TD_FromAToC())
                  {TransitionFromAToC();}
          }

          void TransitionDetection_B()
          {
              // CHECK IF B GOES TO C
              if(TD_FromBToC())
                  {TransitionFromBToC();}
          }

          void TransitionDetection_C()
          {
              // CHECK IF C GOES TO A
              if(TD_FromCToA())
                  {TransitionFromCToA();}
          }

      }
}
