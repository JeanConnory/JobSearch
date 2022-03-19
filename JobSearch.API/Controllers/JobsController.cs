using JobSearch.API.Database;
using JobSearch.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobSearch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        public int numberOfRegistryByPage = 5;

        private readonly JobSearchContext _data;

        public JobsController(JobSearchContext data)
        {
            _data = data;
        }

        [HttpGet]
        public IEnumerable<Job> GetJobs(string word, string cityState, int numberOfPage = 1)
        {
            if (word == null)
                word = String.Empty;
            if(cityState == null)
                cityState = String.Empty;

            return _data.Jobs
                            .Where(a =>
                                 a.PublicationDate >= DateTime.Now.AddDays(-15) &&
                                 a.CityState.ToLower().Contains(cityState.ToLower()) &&
                                (
                                    a.JobTitle.ToLower().Contains(word.ToLower()) ||
                                    a.TecnologyTools.ToLower().Contains(word.ToLower()) ||
                                    a.Company.ToLower().Contains(word.ToLower()))
                                )
                            .Skip(numberOfRegistryByPage * (numberOfPage - 1))
                            .Take(numberOfRegistryByPage)
                            .ToList<Job>();
        }

        [HttpGet("{id}")]
        public IActionResult GetJob(int id)
        {
            var jobDb = _data.Jobs.Find(id);

            if (jobDb == null)
                return NotFound();

            return new JsonResult(jobDb);
        }

        [HttpPost]
        public IActionResult AddJob(Job job)
        {
            job.PublicationDate = DateTime.Now;
            _data.Jobs.Add(job);
            _data.SaveChanges();

            return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
        }
    }
}
