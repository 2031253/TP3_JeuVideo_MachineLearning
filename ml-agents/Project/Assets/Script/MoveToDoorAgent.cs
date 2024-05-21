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

    private bool interrupteurActivé = false;

    private MeshRenderer porteRenderer;  // Renderer pour la porte (pour la modification de couleure)
    private MeshRenderer interrupteurRenderer;  // Renderer pour l'interrupteur (pour la modification de couleure)
   [SerializeField] private MeshRenderer floorRenderer;  // Renderer pour le sol (pour la modification de couleure)

    public override void Initialize()
    {
        porteRenderer = porteOuverte.GetComponent<MeshRenderer>();  // Obtient le MeshRenderer de la porte
        interrupteurRenderer = interrupteur.GetComponent<MeshRenderer>();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Extrait les actions continues
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];


        // Déplacement de l'agent en fonction des actions
        Vector3 moveVector = new Vector3(moveX, 0, moveZ) * speed * Time.deltaTime;
        transform.Translate(moveVector, Space.World);

        // Pénalité à chaque pas pour encourager des solutions rapides
        AddReward(-0.01f);

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
        //floorRenderer.material.color = Color.gray;
        // Réinitialise la position de l'agent et de l'interrupteur
        interrupteurActivé = false;
        transform.localPosition = new Vector3(1f, 7, -2);
        porteRenderer.material.color = Color.red; // Réinitialise la couleur de la porte à rouge
        interrupteurRenderer.material.color = Color.green;
    }


    private void OnTriggerEnter(Collider other)
    {
         if (other.CompareTag("interrupteur") && !interrupteurActivé) // pour qu'il n'aille pas touché plusieurs fois l'interrupteur pour les recompenses
        {
            interrupteurActivé = true;
            AddReward(2f); // Récompense pour avoir touché l'interrupteur
            porteRenderer.material.color = Color.green;   // Change la couleur de la porte à vert pour indiquer l'ouverture
            interrupteurRenderer.material.color = Color.gray; //eteint l'interrupteur
        }
        else if (other.CompareTag("door"))
        {
            if (interrupteurActivé)
            {
                AddReward(10.0f); // Récompense pour réussir à sortir après activation de l'interrupteur
                floorRenderer.material.color = Color.green;
            }
            else
            {
                AddReward(-2f); // Pénalité pour atteindre la porte sans activation de l'interrupteur
                floorRenderer.material.color = Color.blue; // Change la couleur du sol à bleu
            }
            EndEpisode(); // Termine l'épisode
        }
    }

        private void OnTriggerExit(Collider other)
       {

        Debug.Log("Touche");
        if (other.CompareTag("mur"))
        {
            AddReward(-3.0f); // Pénalité pour avoir touché un mur
            floorRenderer.material.color = Color.red; // Change la couleur du sol à rouge
            EndEpisode(); // Termine l'épisode
        }
       
    }


    //pour tester manuellement
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> contActions = actionsOut.ContinuousActions;
        contActions[0] = Input.GetAxisRaw("Horizontal");
        contActions[1] = Input.GetAxisRaw("Vertical");
    }


}
