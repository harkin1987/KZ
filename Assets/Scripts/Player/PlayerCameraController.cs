using PlayerMovement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{

    public Transform m_CamTran;
    [Header("Aiming")]
    [SerializeField] private Camera m_Camera;
    [SerializeField] public MouseLook m_MouseLook = new MouseLook();
    public Transform m_Tran;
    // Start is called before the first frame update
    void Start()
    {
        if (!m_Camera)
            m_Camera = Camera.main;
        m_Tran = transform;
        m_CamTran = m_Camera.transform;
        m_MouseLook.Init(m_Tran, m_CamTran);
    }
    private void Update()
    {
        m_MouseLook.LookRotation(m_Tran, m_CamTran);
    }
}
