using UnityEngine;

namespace PlayerMovement
{
    /// <summary>
    /// This script is used to display real-time statistics to help with tweaking and debugging.
    /// </summary>
    [RequireComponent(typeof(PlayerControllerCC))]
    public class Q3PlayerDebug : MonoBehaviour
    {
        [Tooltip("How many times per second to update stats")]
        [SerializeField] private float m_RefreshRate = 4;

        private int m_FrameCount = 0;
        private float m_Time = 0;
        private float m_FPS = 0;
        private float m_TopSpeed = 0;
        private PlayerControllerCC m_Player;

        private void Start()
        {
            m_Player = GetComponent<PlayerControllerCC>();
        }

        private void LateUpdate()
        {
            // Calculate frames-per-second.
            m_FrameCount++;
            m_Time += Time.deltaTime;
            if (m_Time > 1.0 / m_RefreshRate)
            {
                m_FPS = Mathf.Round(m_FrameCount / m_Time);
                m_FrameCount = 0;
                m_Time -= 1.0f / m_RefreshRate;
            }

            // Calculate top velocity.
            if (m_Player.Speed > m_TopSpeed)
            {
                m_TopSpeed = m_Player.Speed;
            }
        }

        private void OnGUI()
        {
            GUI.Box(new Rect(0, 0, 400, 160),
                "FPS: " + m_FPS + "\n" +
                "Speed: " + Mathf.Round(m_Player.Speed * 100) / 100 + " (ups)\n" +
                "Strafing: " + m_Player.strafing + "\n" +
                "Top: " + Mathf.Round(m_TopSpeed * 100) / 100 + " (ups)\n" + 
                "Horizontal Raw: " + Input.GetAxisRaw("Horizontal") + "\n" +
                "Vertical Raw: " + Input.GetAxisRaw("Vertical") + "\n" +
                "Grounded State: " + m_Player.grounded + "\n" +
                "Grounded Angle: " + m_Player.groundAngle + "\n" +
                "OnSlope: " + m_Player.onSlope + "\n");
        }
    }
}