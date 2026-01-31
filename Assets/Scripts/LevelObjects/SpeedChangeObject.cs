using UnityEngine;

public class SpeedChangeObject : LevelObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int forwardSpeed;
    public int thrustSpeed;

    public override string SerializeExtraData()
    {
        SpeedChangeObjectData data = new SpeedChangeObjectData
        {
            forwardSpeed = forwardSpeed,
            thrustSpeed = thrustSpeed

        };




        return JsonUtility.ToJson(data);
    }

    public override void DeserializeExtraData(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        SpeedChangeObjectData data = JsonUtility.FromJson<SpeedChangeObjectData>(json);

        forwardSpeed = data.forwardSpeed;
        thrustSpeed = data.thrustSpeed;
    }

    public void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.transform.parent.TryGetComponent<PlayerController>(out PlayerController plr))
        {
         
            plr.forwardSpeed = forwardSpeed;
            plr.thrustForce = thrustSpeed;
        }

    }

}
