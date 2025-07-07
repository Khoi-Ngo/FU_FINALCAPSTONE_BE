@@ .. @@
             await _curriculumRepository.UpdateAsync(curriculum);
         }
 
-        public async Task DeleteCurriculumAsync(long id)
+        public async Task<bool> DeleteCurriculumAsync(long id)
         {
             var curriculum = await _curriculumRepository.GetByIdAsync(id);
             if (curriculum == null || curriculum.IsDeleted)
@@ -107,6 +108,7 @@ namespace AISEA.ApiService.BAL.Services.Curriculum
             curriculum.DeletedAt = DateTime.UtcNow;
             
             await _curriculumRepository.UpdateAsync(curriculum);
+            return true;
         }
 
         public async Task AddSubjectToCurriculumAsync(long curriculumId, AddSubjectToCurriculumRequest request)
@@ -155,5 +157,32 @@ namespace AISEA.ApiService.BAL.Services.Curriculum
 
             await _curriculumSubjectRepository.RemoveSubjectFromCurriculumAsync(curriculumId, subjectId);
         }
+        
+        public async Task<bool> CreateCurriculaAsync(List<CreateCurriculumRequest> requests)
+        {
+            foreach (var request in requests)
+            {
+                // Validate program exists
+                var program = await _programRepository.GetByIdAsync(request.ProgramId);
+                if (program == null || program.IsDeleted)
+                {
+                    throw new NotFoundException($"Program with ID {request.ProgramId} not found.");
+                }
+
+                // Check if curriculum code is unique
+                var isCodeUnique = await _curriculumRepository.IsCodeUniqueAsync(request.CurriculumCode);
+                if (!isCodeUnique)
+                {
+                    throw new InvalidUserCreatedException($"Curriculum with code '{request.CurriculumCode}' already exists.");
+                }
+
+                var curriculum = _mapper.Map<DAL.Entities.Curriculum>(request);
+                curriculum.CreatedAt = DateTime.UtcNow;
+                
+                await _curriculumRepository.CreateAsync(curriculum);
+            }
+            
+            return true;
+        }
     }
 }