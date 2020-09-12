﻿using UnityEngine;
using UnityEngine.XR;

namespace ThunderRoad
{

    public class PlayerControllerTest : MonoBehaviour
    {
        public Transform head;
        public float moveSpeed = 4.0f;
        public float turnSpeed = 4.0f;
        public float jumpForce = 20;

        protected new Rigidbody rigidbody;
        protected new CapsuleCollider collider;

        private Vector3 moveDirection = Vector3.zero;

        void Awake()
        {
        Time.fixedDeltaTime = Time.timeScale / XRDevice.refreshRate;
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);
        }

    void FixedUpdate()
    {
        collider.center = new Vector3(this.transform.InverseTransformPoint(head.position).x, 0, this.transform.InverseTransformPoint(head.position).z);
        Vector3 moveDirection = Quaternion.Euler(0, head.transform.rotation.eulerAngles.y, 0) * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection *= moveSpeed;

        if (moveDirection.magnitude < 0.1f) moveDirection = Vector3.zero;

        // Move
        rigidbody.velocity = new Vector3(moveDirection.x, rigidbody.velocity.y, moveDirection.z);

        // Turn
        float axisTurn = Input.GetAxis("Turn");
        if (axisTurn > 0.1f || axisTurn < -0.1f) this.transform.RotateAround(head.position, Vector3.up, axisTurn * turnSpeed);

        // Jump
        if (Input.GetButton("Jump"))
        {
            rigidbody.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }
    }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Zone"))
            {
                Zone zone = other.GetComponent<Zone>();
                if (zone.teleportPlayer)
                {
                    this.transform.position = zone.customTeleportTarget ? zone.customTeleportTarget.position : Level.current.playerStart.position;
                    this.transform.rotation = zone.customTeleportTarget ? zone.customTeleportTarget.rotation : Level.current.playerStart.rotation;
                }
                zone.playerEnterEvent.Invoke(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Zone"))
            {
                Zone zone = other.GetComponent<Zone>();
                zone.playerExitEvent.Invoke(this);
            }
        }
    }
}