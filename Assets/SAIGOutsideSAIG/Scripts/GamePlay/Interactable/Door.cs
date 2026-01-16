using UnityEngine;
namespace Interactable
{
    public class Door : MonoBehaviour
    {
        private Animator animator;
        private int openParamID;
        private const string OPEN_STRING = "Open";
        private void Awake()
        {
            animator = GetComponent<Animator>();
            openParamID = Animator.StringToHash(OPEN_STRING);
        }
        public void TriggerOpenAnimation()
        {

            animator.SetTrigger(openParamID);
        }

    }
}