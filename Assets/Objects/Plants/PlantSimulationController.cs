using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the simulation of plant growth over time,
/// including play/pause/stop mechanics and optional wind effect toggling.
/// </summary>
public class PlantSimulationController : MonoBehaviour
{
    /// <summary>
    /// Time delay (in seconds) between each growth step per plant.
    /// </summary>
    public float growthInterval = 1.0f;

    // Coroutine reference used to stop or restart the simulation.
    private Coroutine simulationCoroutine;

    // State flag to determine if the simulation is currently paused.
    private bool isPaused = false;

    // Toggles wind effect across all applicable components.
    private bool windActive = true;

    // Cached array of all PlantGenerator instances in the scene.
    private PlantGenerator[] plantGenerators;

    /// <summary>
    /// Finds all plant generators in the scene on load.
    /// Ensures the controller has references to all active generators.
    /// </summary>
    void Awake()
    {
        plantGenerators = FindObjectsOfType<PlantGenerator>();
    }

    /// <summary>
    /// Resumes the simulation if it was previously paused.
    /// </summary>
    public void PlaySimulation()
    {
        if (isPaused)
        {
            isPaused = false;
        }
    }

    /// <summary>
    /// Pauses the simulation without resetting progress.
    /// </summary>
    public void PauseSimulation()
    {
        isPaused = true;
    }

    /// <summary>
    /// Stops the simulation and resets internal state.
    /// </summary>
    public void StopSimulation()
    {
        if (simulationCoroutine != null)
        {
            StopCoroutine(simulationCoroutine);
            simulationCoroutine = null;
            isPaused = false;
        }
    }

    /// <summary>
    /// Stops the current simulation, clears all plant visuals,
    /// and restarts the simulation from scratch.
    /// </summary>
    public IEnumerator RestartSimulation()
    {
        StopSimulation();
        // Reset each plant one at a time
        foreach (var plant in plantGenerators)
        {
            yield return StartCoroutine(plant.ResetPlant());
        }
        // Refresh plant list in case new ones were created
        plantGenerators = FindObjectsOfType<PlantGenerator>();
        PlaySimulation();
    }

    /// <summary>
    /// Starts the growth simulation for all plants concurrently.
    /// Each plant simulates growth through multiple iterations.
    /// </summary>
    private IEnumerator RunSimulation()
    {
        foreach (var plant in plantGenerators)
        {
            StartCoroutine(SimulatePlantGrowth(plant));
        }
        yield return null;
        simulationCoroutine = null;
    }

    /// <summary>
    /// Runs the plant growth process over a series of iterations,
    /// applying a delay between steps to simulate gradual growth.
    /// </summary>
    /// <param name="plant">The PlantGenerator instance to simulate.</param>
    private IEnumerator SimulatePlantGrowth(PlantGenerator plant)
    {
        int maxIterations = plant.iterations;
        for (int i = 0; i <= maxIterations; i++)
        {
            // Pause support: wait until resumed
            while (isPaused)
            {
                yield return null;
            }
            // Trigger growth step
            plant.SimulateStep(i);
            // Wait before the next step
            yield return new WaitForSeconds(growthInterval);
        }
    }

    /// <summary>
    /// Toggles wind simulation on or off for all WindEffect components in the scene.
    /// </summary>
    public void ToggleWind()
    {
        windActive = !windActive;
        WindEffect[] windComponents = FindObjectsOfType<WindEffect>();
        foreach (var wind in windComponents)
        {
            wind.enabled = windActive;
        }
    }

    /// <summary>
    /// Immediately grows multiple plants up to a given number of iterations.
    /// </summary>
    /// <param name="iterations">Number of growth steps to apply per plant.</param>
    public IEnumerator MultiplePlantsGrowthRoutine(int iterations)
    {
        PlantGenerator[] plantsToGrow = FindObjectsOfType<PlantGenerator>();
        foreach (var plant in plantsToGrow)
        {
            plant.SetIterations(iterations);
            plant.GeneratePlant();
            yield return null; // Yield per plant to avoid frame hitching
        }
    }
}
