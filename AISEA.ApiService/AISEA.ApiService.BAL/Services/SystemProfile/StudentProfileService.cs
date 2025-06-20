using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.SystemProfile;
using AutoMapper;

namespace AISEA.ApiService.BAL.Services.SystemProfile
{
    public class StudentProfileService
    {
        private readonly IMapper _mapper;
        private readonly StudentProfileRepository _studentProfileRepository;

        public StudentProfileService(IMapper mapper, StudentProfileRepository studentProfileRepository)
        {
            _mapper = mapper;
            _studentProfileRepository = studentProfileRepository;
        }

        public async Task CreateAsync(CreateStudentProfileRequest request)
        {
            var studentProfile = _mapper.Map<StudentProfile>(request);
            await _studentProfileRepository.CreateAsync(studentProfile);
        }
    }
}