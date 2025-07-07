@@ .. @@
     public class SubjectService
     {
         private readonly SubjectRepository _subjectRepository;
         private readonly SubjectPrerequisiteRepository _prerequisiteRepository;
         private readonly IMapper _mapper;
 
@@ .. @@
 
         public async Task<GetSubjectResponse> GetSubjectByIdAsync(long id)
         {
-            var subject = await _subjectRepository.GetByIdAsync(id);
+            var subject = await _subjectRepository.GetByIdWithPrerequisitesAsync(id);
             if (subject == null || subject.IsDeleted)
             {
                 throw new NotFoundException("Subject not found.");
             }
 
-            return _mapper.Map<GetSubjectResponse>(subject);
+            var response = _mapper.Map<GetSubjectResponse>(subject);
+            
+            // Map prerequisites
+            if (subject.Prerequisites != null && subject.Prerequisites.Any())
+            {
+                response.Prerequisites = _mapper.Map<List<GetSubjectResponse>>(
+                    subject.Prerequisites.Select(p => p.PrerequisiteSubject).ToList());
+            }
+            
+            return response;
         }
 
@@ .. @@
             await _prerequisiteRepository.RemovePrerequisiteAsync(subjectId, prerequisiteSubjectId);
             return true;
         }
+        
+        public async Task<bool> CreateSubjectsAsync(List<CreateSubjectRequest> requests)
+        {
+            foreach (var request in requests)
+            {
+                var existingSubject = await _subjectRepository.GetByCodeAsync(request.SubjectCode);
+                if (existingSubject != null)
+                {
+                    throw new InvalidUserCreatedException($"Subject with code '{request.SubjectCode}' already exists.");
+                }
+
+                var subject = _mapper.Map<DAL.Entities.Subject>(request);
+                subject.CreatedAt = DateTime.UtcNow;
+                
+                await _subjectRepository.CreateAsync(subject);
+            }
+            
+            return true;
+        }
     }
 }