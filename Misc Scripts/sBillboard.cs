using System.Collections;
using UnityEngine;

namespace Assets.Misc_Scripts
{
    public class sBillboard : MonoBehaviour
    {
        private GameObject cam;

        void Start()
        {
            cam = GameObject.Find("MagicCamera");
        }

        void Update()
        {
            transform.forward = cam.transform.forward;
        }
    }
}