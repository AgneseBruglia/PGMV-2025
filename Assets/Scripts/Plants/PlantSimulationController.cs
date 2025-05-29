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
            isPaused = false;
            simulationCoroutine = StartCoroutine(RunSimulation());
        }
        else if (isPaused)
        {
            isPaused = false; // semplicemente sblocca la pausa senza ricominciare la coroutine
        }
    }

    public void PauseSimulation()
    {
        isPaused = true;
        // Non stoppare la coroutine per permettere il resume
        // StopSimulation();
    }

    public void StopSimulation()
    {
        if (simulationCoroutine != null)
        {
            StopCoroutine(simulationCoroutine);
            simulationCoroutine = null;
            isPaused = false;
        }
    }

    public IEnumerator RestartSimulation()
    {
        StopSimulation();

        foreach (var plant in plantGenerators)
        {
            yield return StartCoroutine(plant.ResetPlant());
        }

        plantGenerators = FindObjectsOfType<PlantGenerator>();
        Debug.Log($"PlantGenerators count after restart: {plantGenerators.Length}");
        
        PlaySimulation();
    }

    private IEnumerator RunSimulation()
    {
        foreach (var plant in plantGenerators)
        {
            StartCoroutine(SimulatePlantGrowth(plant));
        }

        yield return null;

        simulationCoroutine = null;
    }

    private IEnumerator SimulatePlantGrowth(PlantGenerator plant)
    {
        int maxIterations = plant.iterations;
        for (int i = 0; i <= maxIterations; i++)
        {
            while (isPaused)
            {
                yield return null;
            }

            plant.SimulateStep(i);
            yield return new WaitForSeconds(growthInterval);
        }
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
