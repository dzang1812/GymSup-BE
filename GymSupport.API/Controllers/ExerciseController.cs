using GymSupport.Repository.Interfaces;
using GymSupport.Repository.Models.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ExerciseController : ControllerBase
{
    private readonly IExerciseRepository _exerciseRepository;

    public ExerciseController(
        IExerciseRepository exerciseRepository)
    {
        _exerciseRepository = exerciseRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _exerciseRepository.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var exercise =
            await _exerciseRepository.GetByIdAsync(id);

        if (exercise == null)
            return NotFound();

        return Ok(exercise);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] Exercise exercise)
    {
        await _exerciseRepository.CreateAsync(exercise);

        return Ok(exercise);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] Exercise request)
    {
        var exercise =
            await _exerciseRepository.GetByIdAsync(id);

        if (exercise == null)
            return NotFound();

        exercise.Name = request.Name;
        exercise.TargetMuscles = request.TargetMuscles;
        exercise.Equipment = request.Equipment;
        exercise.Difficulty = request.Difficulty;
        exercise.ImageUrl = request.ImageUrl;
        exercise.VideoUrl = request.VideoUrl;

        await _exerciseRepository.UpdateAsync(exercise);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _exerciseRepository.DeleteAsync(id);

        return NoContent();
    }
}