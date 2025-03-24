using UnityEngine;
using Mirror;
using TMPro;
using Steamworks;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerName;
    [SerializeField]
    private float moveSpeed = 5f;
    void Start()
    {
        // �����ϸ� ���� �г����� �÷��̾� �̸����� ����
        playerName.text = SteamFriends.GetPersonaName();
        transform.position = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0);
    }
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        // �÷��̾� �̵�
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, v, 0).normalized;

        transform.Translate(move * moveSpeed * Time.deltaTime);
    }
}