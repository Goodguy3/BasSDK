﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace ThunderRoad
{

    public class PlayerGrabTest : MonoBehaviour
    {
        public XRNode xrNode;
        protected Rigidbody rb;
        protected FixedJoint fixedJoint;
        protected Rigidbody grabbedRb;
        protected bool orgKinematic;
        protected InputDevice device;

        protected bool primaryPressState;
        protected bool secondaryPressState;
        protected bool gripPressState;

        private void Awake()
        {
            rb = this.GetComponent<Rigidbody>();
            if (!rb) rb = this.gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        private void Start()
        {
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesAtXRNode(xrNode, devices);

            if (devices.Count == 1)
            {
                device = devices[0];
                Debug.Log(string.Format("Device name '{0}' with role '{1}'", device.name, device.role.ToString()));
            }
            else if (devices.Count > 1)
            {
                Debug.Log("Found more than one left hand!");
            }
        }

        void Update()
        {

#if DUNGEN
            if (xrNode == XRNode.LeftHand)
            {
                if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryPressed))
                {
                    if (primaryPressed)
                    {
                        if (!primaryPressState)
                        {
                            PlayerControllerTest playerControllerTest = this.GetComponentInParent<PlayerControllerTest>();
                            DunGen.AdjacentRoomCulling adjacentRoomCulling = playerControllerTest.head.gameObject.GetComponent<DunGen.AdjacentRoomCulling>();
                            adjacentRoomCulling.enabled = !adjacentRoomCulling.enabled;
                            primaryPressState = true;
                        }
                    }
                    else
                    {
                        if (primaryPressState)
                        {
                            primaryPressState = false;
                        }
                    }
                }
                if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryPressed))
                {
                    if (secondaryPressed)
                    {
                        if (!secondaryPressState)
                        {
                            Level level = GameObject.FindObjectOfType<Level>();
                            if (level && level.dungeonGenerator)
                            {
                                level.GenerateDungeon();
                            }
                            secondaryPressState = true;
                        }
                    }
                    else
                    {
                        if (secondaryPressState)
                        {
                            secondaryPressState = false;
                        }
                    }
                }
            }
#endif
            if (device.TryGetFeatureValue(CommonUsages.gripButton, out bool gripPressed))
            {
                if (gripPressed)
                {
                    if (!gripPressState)
                    {
                        if (!grabbedRb)
                        {
                            Collider[] colliders = Physics.OverlapSphere(this.transform.position, 0.05f);
                            float closestDistanceSqr = Mathf.Infinity;
                            Collider nearestCollider = null;
                            foreach (Collider collider in colliders)
                            {
                                if (collider.attachedRigidbody)
                                {
                                    float dSqrToTarget = (collider.ClosestPoint(this.transform.position) - this.transform.position).sqrMagnitude;
                                    if (dSqrToTarget < closestDistanceSqr)
                                    {
                                        closestDistanceSqr = dSqrToTarget;
                                        nearestCollider = collider;
                                    }
                                }
                            }
                            if (nearestCollider)
                            {
                                if (nearestCollider.attachedRigidbody)
                                {
                                    //orgKinematic = nearestCollider.attachedRigidbody.isKinematic;
                                    grabbedRb = nearestCollider.attachedRigidbody;
                                    fixedJoint = rb.gameObject.AddComponent<FixedJoint>();
                                    fixedJoint.connectedBody = grabbedRb;
                                    this.GetComponent<MeshRenderer>().enabled = false;
                                }
                            }
                        }
                        gripPressState = true;
                    }
                }
                else
                {
                    if (gripPressState)
                    {
                        if (grabbedRb)
                        {
                            if (fixedJoint) Destroy(fixedJoint);
                            grabbedRb = null;
                            this.GetComponent<MeshRenderer>().enabled = true;
                        }
                        gripPressState = false;
                    }
                }
            }
        }
    }
}