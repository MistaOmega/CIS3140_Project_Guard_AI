using UnityEngine;

namespace worldspace
{
    public enum State
    {
        LOCKED,
        UNLOCKED
    }


    public class Lockable : MonoBehaviour
    {
        [SerializeField] private float timer = 5.0f;
        private bool _isColliding;
        private State _state;


        private void Update()
        {
            if (_isColliding && timer > 0) timer -= Time.deltaTime;
        }


        #region Triggers

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (_state == State.LOCKED) _isColliding = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player") || !_isColliding) return;
            if (timer <= 0) _state = State.UNLOCKED;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            Debug.Log("Player exited door region");
            _isColliding = false;
            timer = 5.0f;
        }

        #endregion
    }
}