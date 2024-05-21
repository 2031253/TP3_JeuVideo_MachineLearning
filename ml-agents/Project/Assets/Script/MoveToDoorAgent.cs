using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.Sentis.Layers;
using UnityEngine;

public class MoveToDoorAgent : Agent
{
    //les buts 
    [SerializeField] private Transform porteOuverte;
    [SerializeField] private Transform interrupteur;


    [SerializeField] private float speed = 6f;  // Vitesse de déplacement de l'agent

    private MeshRenderer porteRenderer;  // Renderer pour la porte (pour la modification de couleure)

    public override void Initialize()
    {
        porteRenderer = porteOuverte.GetComponent<MeshRenderer>();  // Obtient le MeshRenderer de la porte
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Extrait les actions continues
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];


        // Déplacement de l'agent en fonction des actions
        Vector3 moveVector = new Vector3(moveX, 0, moveZ) * speed * Time.deltaTime;
        transform.Translate(moveVector, Space.World);

        Debug.Log(actions.DiscreteActions[0]);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //vu qu'on a 3 vecteurs d'observation , on va mettre la space siza à 9 (3*3)
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(porteOuverte.localPosition);
        sensor.AddObservation(interrupteur.localPosition);
    }

    public override void OnEpisodeBegin()
    {
        // Réinitialise la position de l'agent et de l'interrupteur
        transform.localPosition = new Vector3(1f, 0.67f, -2);
        porteRenderer.material.color = Color.red; // Réinitialise la couleur de la porte à rouge
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("mur"))
        {
            AddReward(-1.0f); // Pénalité pour avoir touché un mur
            EndEpisode(); // Termine l'épisode
        }
        else if (other.CompareTag("interrupteur"))
        {
            AddReward(0.1f); // Récompense pour avoir touché l'interrupteur
            porteRenderer.material.color = Color.green;   // Change la couleur de la porte à vert pour indiquer l'ouverture
        }
        else if (other.CompareTag("door"))
        {
            AddReward(1.0f); // Grande récompense pour avoir réussi à sortir
            EndEpisode(); // Termine l'épisode
        }
    }

  /*  private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Detected with: " + collision.gameObject.name); // Log pour voir avec quel objet la collision se produit

        if (collision.gameObject.CompareTag("mur"))
        {
            Debug.Log("Collided with Wall"); // Log spécifique pour la collision avec un mur
            AddReward(-1.0f); // Pénalité pour avoir touché un mur
            EndEpisode(); // Termine l'épisode
        }
        else if (collision.gameObject.CompareTag("interrupteur"))
        {
            Debug.Log("Collided with Switch"); // Log spécifique pour la collision avec l'interrupteur
            AddReward(0.1f); // Récompense pour avoir touché l'interrupteur
            porteRenderer.material.color = Color.green; // Change la couleur de la porte à vert pour indiquer l'ouverture
        }
        else if (collision.gameObject.CompareTag("door"))
        {
            Debug.Log("Collided with Door"); // Log spécifique pour la collision avec la porte
            AddReward(1.0f); // Grande récompense pour avoir réussi à sortir
            EndEpisode(); // Termine l'épisode
        }
    }*/

    //pour tester manuellement
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> contActions = actionsOut.ContinuousActions;
        contActions[0] = Input.GetAxisRaw("Horizontal");
        contActions[1] = Input.GetAxisRaw("Vertical");
    }


}
