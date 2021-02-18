using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace VirtualPhenix.DLLExample
{
    /*
     * Assuming a DLL called Example.dll in C++
     * With:
     * -  Init method call: initialization()
     * -  method called DebugLog() that returns a string
     * -  metod called Add(int _val1, int _val2) that returns the sum
     * -  Stop method called Stop(); that stops the DLL from running
     */
    /// <summary>
    /// Dll loader example for unity loading "Example.dll" in Assets/plugins
    /// </summary>
    public class VP_DLLHandlerExample : VP_Monobehaviour
    {
        #region Variables & Properties
        /// <summary>
        /// Pointer of the DLL, stored for deallocation
        /// </summary>
        private IntPtr m_pointerToDLL;
        /// <summary>
        /// DLL Name-> no need the .dll as unity handles that directly
        /// </summary>
        [SerializeField] private string m_dllName = "Example";
        /// <summary>
        /// Debug log all the time
        /// </summary>
        [SerializeField] private bool m_debugLog = false;
        /// <summary>
        /// Getter for pointer
        /// </summary>
        public IntPtr Pointer { get { return m_pointerToDLL; } }
        /// <summary>
        /// Check allocated memory in pointer
        /// </summary>
        public bool IsPointerAllocated
        {
            get
            {
                return m_pointerToDLL != IntPtr.Zero;
            }
        }
        #endregion

        #region Plugin methods
        /// <summary>
        /// Delegate of the Init method. When calling it in the DLL, this will be called.
        /// Both must share name as reflection will handle it as string
        /// </summary>
        private delegate void Initialization();
        /// <summary>
        /// Stopping method
        /// </summary>
        private delegate void Stop();
        /// <summary>
        /// THis method from the DLL returns a string.
        /// </summary>
        /// <returns></returns>
        private delegate string DebugLog();
        /// <summary>
        /// Add m,e
        /// </summary>
        /// <returns></returns>
        private delegate int Add(int _value1, int _value2);
        #endregion

        #region MonoBehaviour Methods

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            AllocateDLL();
            if (!IsPointerAllocated)
            {
                Debug.LogError("Failed to load native library");
            }
            else
            {
                Debug.Log("Native library loaded correctly");
                InitializeDLL();
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (IsPointerAllocated)
            {
                // Debug every frame if 
                if (m_debugLog)
                {
                    Debug.Log("Log from DLL: " + GetLogFromDLL());
                }

                // When pressing enter, the DLL returns the sum of 2 random numbers and we debug.Log
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    Debug.Log(CheckAdd(UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(0, 50)));
                }
            }

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (IsPointerAllocated)
                DeallocateDLL();
        }
        #endregion

        #region Other Methods
        /// <summary>
        /// Call Add(int, int) method from DLL, it returns the sum of both as integer
        /// </summary>
        /// <param name="_value1"></param>
        /// <param name="_value2"></param>
        /// <returns></returns>
        public int CheckAdd(int _value1, int _value2)
        {
            /// First the returning value from DLL method, then the casting type. Not needed if void
            return VP_DLLHandler.Invoke<int, Add>(m_pointerToDLL, _value1, _value2);
        }
        /// <summary>
        /// Call DebugLog(string) method from DLL
        /// </summary>
        /// <returns></returns>
        public string GetLogFromDLL()
        {
            /// First the returning valuefrom DLL method, then the casting type. Not needed if void
            return VP_DLLHandler.Invoke<string, DebugLog>(m_pointerToDLL);
        }
        /// <summary>
        /// Calling the DLL and saving the pointer to it
        /// </summary>
        public void AllocateDLL()
        {
            // Load the dynamic library
            m_pointerToDLL = VP_DLLHandler.LoadLibrary(m_dllName);
        }
        /// <summary>
        /// Calling the DLL to free the allcoated memory
        /// </summary>
        public void DeallocateDLL()
        {
            // Deallocate the dynamic library
            VP_DLLHandler.Invoke<Stop>(m_pointerToDLL);
            VP_DLLHandler.FreeLibrary(m_pointerToDLL);
        }

        /// <summary>
        /// First needed method in our DLL program (Prepare/Start/init);
        /// </summary>
        public void InitializeDLL()
        {
            /// Init_> Call callibration
            VP_DLLHandler.Invoke<Initialization>(m_pointerToDLL);
        }

        #endregion
    }
}
