using System.Collections;
using UnityEngine;

public class PlantSimulationController : MonoBehaviour
{
    // Time between every step of the growth
    public float growthInterval = 1.0f;
    private Coroutine simulationCoroutine;
    private bool isPaused = false;
    private bool windActive = true;

    private PlantGenerator[] plantGenerators;

    void Awake()
    {
        plantGenerators = FindObjectsOfType<PlantGenerator>();
    }

    public void PlaySimulation()
    {
        if (simulationCoroutine == null)
        {
            simulationCoroutine = StartCoroutine(RunSimulation());
        }
        else if (isPaused)
        {
            isPaused = false;
        }
    }

    public void PauseSimulation()
    {
        isPaused = true;
    }

    public void RestartSimulation()
    {
        StopSimulation();
        foreach (var plant in plantGenerators)
        {
            plant.ResetPlant();
        }
        PlaySimulation();
    }

    public void StopSimulation()
    {
        if (simulationCoroutine != null)
        {
            StopCoroutine(simulationCoroutine);
            simulationCoroutine = null;
        }
    }

    private IEnumerator RunSimulation()
    {
        int totalIterations = 0;

        foreach (var plant in plantGenerators)
        {
            if (plant.iterations > totalIterations)
                totalIterations = plant.iterations;
        }

        for (int i = 0; i <= totalIterations; i++)
        {
            if (!isPaused)
            {
                foreach (var plant in plantGenerators)
                {
                    plant.SimulateStep(i);
                }
            }

            yield return new WaitForSeconds(growthInterval);
        }

        simulationCoroutine = null;
    }
    
    public void ToggleWind()
    {
        windActive = !windActive;

        WindEffect[] windComponents = FindObjectsOfType<WindEffect>();
        foreach (var wind in windComponents)
        {
            wind.enabled = windActive;
        }
    }
}
