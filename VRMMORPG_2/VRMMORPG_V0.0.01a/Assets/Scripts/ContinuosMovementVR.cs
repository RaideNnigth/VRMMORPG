using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;

public class ContinuosMovementVR : MonoBehaviour
{
    [SerializeField]
    private XRNode inputSource;

    [SerializeField]
    private XROrigin rig;

    [SerializeField]
    private CharacterController character;

    [SerializeField]
    private float speed = 1f;

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private float fallingSpeed;
    
    [SerializeField]
    private float gravity;

    private Vector2 inputAxis;
    private float heightOffset = 0.2f;

    void Start()
    {
        character = GetComponent<CharacterController>();
        rig = GetComponent<XROrigin>();
    }

    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
    }
    private void FixedUpdate()
    {
        Quaternion headYaw = Quaternion.Euler(x: 0, rig.Camera.transform.eulerAngles.y, z: 0);
        Vector3 direction = headYaw * new Vector3(inputAxis.x, y:0, inputAxis.y);
        character.Move(direction * speed * Time.deltaTime);
        bool isGrounded = CheckIfGrounded();
        if (isGrounded)
            fallingSpeed = 0f;
        else
            fallingSpeed += gravity * Time.fixedDeltaTime;
        character.Move(Vector3.up * fallingSpeed * Time.fixedDeltaTime);
        CapsuleFollowHeadSet();
    }

    bool CheckIfGrounded() 
    {
        Vector3 rayStart = transform.TransformPoint(character.center);
        float rayLength = character.center.y + 0.01f;
        bool hasHit = Physics.SphereCast(rayStart, character.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer);
        return hasHit;    
    }

    void CapsuleFollowHeadSet()
    {
        character.height = rig.CameraInOriginSpaceHeight + heightOffset;
        Vector3 capsuleCenter = transform.InverseTransformPoint(rig.Camera.transform.position);
        character.center = new Vector3(capsuleCenter.x, character.height / 2 + character.skinWidth, capsuleCenter.z);
    }
}