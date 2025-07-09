using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Requests.Program;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.Program;
using AISEA.ApiService.SHARED.Exceptions;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISEA.ApiService.BAL.Services.Program
{
    public class ProgramService
    {
        private readonly ProgramRepository _programRepository;
        private readonly IMapper _mapper;

        public ProgramService(ProgramRepository programRepository, IMapper mapper)
        {
            _programRepository = programRepository;
            _mapper = mapper;
        }

        public async Task<long> CreateProgramAsync(CreateProgramRequest request)
        {
            // Check if program code is unique
            var existingProgram = await _programRepository.GetByCodeAsync(request.ProgramCode);
            if (existingProgram != null)
            {
                throw new InvalidUserCreatedException($"Program with code '{request.ProgramCode}' already exists.");
            }

            var program = _mapper.Map<DAL.Entities.Program>(request);
            program.CreatedAt = DateTime.UtcNow;

            await _programRepository.CreateAsync(program);
            return program.Id;
        }

        public async Task<bool> CreateProgramsAsync(List<CreateProgramRequest> requests)
        {
            foreach (var request in requests)
            {
                // Check if program code is unique
                var existingProgram = await _programRepository.GetByCodeAsync(request.ProgramCode);
                if (existingProgram != null)
                {
                    throw new InvalidUserCreatedException($"Program with code '{request.ProgramCode}' already exists.");
                }

                var program = _mapper.Map<DAL.Entities.Program>(request);
                program.CreatedAt = DateTime.UtcNow;

                await _programRepository.CreateAsync(program);
            }
            return true;
        }

        public async Task<PagedResult<GetProgramResponse>> GetProgramsPagedAsync(PaginationRequest request, string? search = null)
        {
            var (programs, totalCount) = await _programRepository.GetPagedAsync(request.PageNumber, request.PageSize, search);

            return new PagedResult<GetProgramResponse>
            {
                Items = _mapper.Map<List<GetProgramResponse>>(programs),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<List<GetProgramResponse>> GetAllActiveProgramsAsync()
        {
            var programs = await _programRepository.GetAllActiveAsync();
            return _mapper.Map<List<GetProgramResponse>>(programs);
        }

        public async Task<GetProgramResponse> GetProgramByIdAsync(long id)
        {
            var program = await _programRepository.GetByIdAsync(id);
            if (program == null || program.IsDeleted)
            {
                throw new NotFoundException("Program not found.");
            }

            return _mapper.Map<GetProgramResponse>(program);
        }

        public async Task UpdateProgramAsync(long id, UpdateProgramRequest request)
        {
            var program = await _programRepository.GetByIdAsync(id);
            if (program == null || program.IsDeleted)
            {
                throw new NotFoundException("Program not found.");
            }

            // Check if program code is being changed and if it conflicts with existing
            if (program.ProgramCode != request.ProgramCode)
            {
                var existingProgram = await _programRepository.GetByCodeAsync(request.ProgramCode);
                if (existingProgram != null && existingProgram.Id != id)
                {
                    throw new InvalidUserCreatedException($"Program with code '{request.ProgramCode}' already exists.");
                }
            }

            _mapper.Map(request, program);
            program.UpdatedAt = DateTime.UtcNow;

            await _programRepository.UpdateAsync(program);
        }

        public async Task<bool> DeleteProgramAsync(long id)
        {
            var program = await _programRepository.GetByIdAsync(id);
            if (program == null || program.IsDeleted)
            {
                throw new NotFoundException("Program not found.");
            }

            // Check if program has curricula
            var hasCurricula = await _programRepository.HasCurriculaAsync(id);
            if (hasCurricula)
            {
                throw new InvalidUserCreatedException("Cannot delete program that has curricula. Please remove all curricula first.");
            }

            program.IsDeleted = true;
            program.DeletedAt = DateTime.UtcNow;

            await _programRepository.UpdateAsync(program);
            return true;
        }
    }
}
