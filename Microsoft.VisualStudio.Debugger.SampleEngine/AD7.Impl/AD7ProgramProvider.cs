using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Debugger.Interop;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Debugger.SampleEngine
{
    // This class implments IDebugProgramProvider2. 
    // This registered interface allows the session debug manager (SDM) to obtain information about programs 
    // that have been "published" through the IDebugProgramPublisher2 interface.
    [ComVisible(true)]
    [Guid("FF3E23A2-DA7E-4fa7-AF47-6EDEDE4E922E")]
    public class AD7ProgramProvider : IDebugProgramProvider2
    {
        public AD7ProgramProvider()
        {
        }

        #region IDebugProgramProvider2 Members

        // Obtains information about programs running, filtered in a variety of ways.
        int IDebugProgramProvider2.GetProviderProcessData(enum_PROVIDER_FLAGS Flags, IDebugDefaultPort2 port, AD_PROCESS_ID ProcessId, CONST_GUID_ARRAY EngineFilter, PROVIDER_PROCESS_DATA[] processArray)
        {
            processArray[0] = new PROVIDER_PROCESS_DATA();
           
            if (EngineUtils.IsFlagSet((uint)Flags, (int)enum_PROVIDER_FLAGS.PFLAG_GET_PROGRAM_NODES))
            {
                // The debugger is asking the engine to return the program nodes it can debug. The 
                // sample engine claims that it can debug all processes, and returns exsactly one
                // program node for each process. A full-featured debugger may wish to examine the
                // target process and determine if it understands how to debug it.

                IDebugProgramNode2 node = (IDebugProgramNode2)(new AD7ProgramNode((int)ProcessId.dwProcessId));             

                IntPtr[] programNodes = { Marshal.GetComInterfaceForObject(node, typeof(IDebugProgramNode2)) };

                IntPtr destinationArray = Marshal.AllocCoTaskMem(IntPtr.Size * programNodes.Length);                
                Marshal.Copy(programNodes, 0, destinationArray, programNodes.Length);

                processArray[0].Fields = enum_PROVIDER_FIELDS.PFIELD_PROGRAM_NODES;
                processArray[0].ProgramNodes.Members = destinationArray;
                processArray[0].ProgramNodes.dwCount = (uint)programNodes.Length;

                return Constants.S_OK;
            }

            return Constants.S_FALSE;
        }

        // Gets a program node, given a specific process ID.
        int IDebugProgramProvider2.GetProviderProgramNode(enum_PROVIDER_FLAGS Flags, IDebugDefaultPort2 port, AD_PROCESS_ID ProcessId, ref Guid guidEngine, ulong programId, out IDebugProgramNode2 programNode)
        {
            // This method is used for Just-In-Time debugging support, which this program provider does not support
            programNode = null;
            return Constants.E_NOTIMPL;
        }

        // Establishes a locale for any language-specific resources needed by the DE. This engine only supports Enu.
        int IDebugProgramProvider2.SetLocale(ushort wLangID)
        {           
            return Constants.S_OK;
        }

        // Establishes a callback to watch for provider events associated with specific kinds of processes
        int IDebugProgramProvider2.WatchForProviderEvents(enum_PROVIDER_FLAGS Flags, IDebugDefaultPort2 port, AD_PROCESS_ID ProcessId, CONST_GUID_ARRAY EngineFilter, ref Guid guidLaunchingEngine, IDebugPortNotify2 ad7EventCallback)
        {
            // The sample debug engine is a native debugger, and can therefore always provide a program node
            // in GetProviderProcessData. Non-native debuggers may wish to implement this method as a way
            // of monitoring the process before code for their runtime starts. For example, if implementing a 
            // 'foo script' debug engine, one could attach to a process which might eventually run 'foo script'
            // before this 'foo script' started.
            //
            // To implement this method, an engine would monitor the target process and call AddProgramNode
            // when the target process started running code which was debuggable by the engine. The 
            // enum_PROVIDER_FLAGS.PFLAG_ATTACHED_TO_DEBUGGEE flag indicates if the request is to start
            // or stop watching the process.
            
            return Constants.S_OK;
        }

        #endregion
    }
}
