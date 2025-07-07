@@ .. @@
             await _sessionRepository.CreateAsync(session);
             return session.Id;
         }
+        
+        public async Task<bool> CreateSyllabusAssessmentsAsync(List<CreateSyllabusAssessmentRequest> requests)
+        {
+            foreach (var request in requests)
+            {
+                var syllabus = await _syllabusRepository.GetByIdAsync(request.SyllabusId);
+                if (syllabus == null || syllabus.IsDeleted)
+                {
+                    throw new NotFoundException($"Syllabus with ID {request.SyllabusId} not found.");
+                }
+
+                var assessment = _mapper.Map<DAL.Entities.SyllabusAssessment>(request);
+                assessment.CreatedAt = DateTime.UtcNow;
+                
+                await _assessmentRepository.CreateAsync(assessment);
+            }
+            return true;
+        }
+        
+        public async Task<bool> CreateSyllabusLearningMaterialsAsync(List<CreateSyllabusLearningMaterialRequest> requests)
+        {
+            foreach (var request in requests)
+            {
+                var syllabus = await _syllabusRepository.GetByIdAsync(request.SyllabusId);
+                if (syllabus == null || syllabus.IsDeleted)
+                {
+                    throw new NotFoundException($"Syllabus with ID {request.SyllabusId} not found.");
+                }
+
+                var material = _mapper.Map<DAL.Entities.SyllabusLearningMaterial>(request);
+                material.CreatedAt = DateTime.UtcNow;
+                
+                await _materialRepository.CreateAsync(material);
+            }
+            return true;
+        }
+        
+        public async Task<bool> CreateSyllabusLearningOutcomesAsync(List<CreateSyllabusLearningOutcomeRequest> requests)
+        {
+            foreach (var request in requests)
+            {
+                var syllabus = await _syllabusRepository.GetByIdAsync(request.SyllabusId);
+                if (syllabus == null || syllabus.IsDeleted)
+                {
+                    throw new NotFoundException($"Syllabus with ID {request.SyllabusId} not found.");
+                }
+
+                var existingOutcome = await _outcomeRepository.GetByCodeAsync(request.SyllabusId, request.OutcomeCode);
+                if (existingOutcome != null)
+                {
+                    throw new InvalidUserCreatedException($"Learning outcome with code '{request.OutcomeCode}' already exists for this syllabus.");
+                }
+
+                var outcome = _mapper.Map<DAL.Entities.SyllabusLearningOutcome>(request);
+                outcome.CreatedAt = DateTime.UtcNow;
+                
+                await _outcomeRepository.CreateAsync(outcome);
+            }
+            return true;
+        }
+        
+        public async Task<bool> CreateSyllabusSessionsAsync(List<CreateSyllabusSessionRequest> requests)
+        {
+            foreach (var request in requests)
+            {
+                var syllabus = await _syllabusRepository.GetByIdAsync(request.SyllabusId);
+                if (syllabus == null || syllabus.IsDeleted)
+                {
+                    throw new NotFoundException($"Syllabus with ID {request.SyllabusId} not found.");
+                }
+
+                var session = _mapper.Map<DAL.Entities.SyllabusSession>(request);
+                session.CreatedAt = DateTime.UtcNow;
+                
+                await _sessionRepository.CreateAsync(session);
+            }
+            return true;
+        }
 
         public async Task MapSessionToOutcomeAsync(long sessionId, long outcomeId)
         {