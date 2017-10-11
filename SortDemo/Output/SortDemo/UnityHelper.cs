using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SortDemo.UnityInterop
{
    public class UnityHelper
    {
        public void PlayAudio(byte[] bytes)
        {
            if (UnityPlayer.AppCallbacks.Instance.IsInitialized())
            {
                UnityPlayer.AppCallbacks.Instance.InvokeOnAppThread(new UnityPlayer.AppCallbackItem(() =>
                {
                    var communicationManager = GameObject.Find("CommunicationManager");
                    if (communicationManager == null)
                    {
                        throw new Exception("CommunicationManager not found");
                    }

                    communicationManager.BroadcastMessage("PlayAudio", bytes);

                    //script.PlayAudio(bytes);
                    var i = 0;

                }), false);
            }
        }

    }
}
