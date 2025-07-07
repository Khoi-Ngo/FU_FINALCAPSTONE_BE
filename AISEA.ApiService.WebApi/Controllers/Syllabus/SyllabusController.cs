@@ .. @@
         /// Creates a new syllabus (Academic Staff only)
         /// </summary>
         [HttpPost]
-        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
         public async Task<IActionResult> CreateSyllabus([FromBody] CreateSyllabusRequest request)
         {
             var syllabusId = await _syllabusService.CreateSyllabusAsync(request);
@@ -33,7 +33,7 @@ namespace AISEA.ApiService.WebApi.Controllers.Syllabus
         /// Updates an existing syllabus (Academic Staff only)
         /// </summary>
         [HttpPut("{id}")]
-        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
         public async Task<IActionResult> UpdateSyllabus(long id, [FromBody] UpdateSyllabusRequest request)
         {
             await _syllabusService.UpdateSyllabusAsync(id, request);
@@ -44,7 +44,7 @@ namespace AISEA.ApiService.WebApi.Controllers.Syllabus
         /// Deletes a syllabus (Admin only)
         /// </summary>
         [HttpDelete("{id}")]
-        [PermissionAuthorize(1)] // Admin only
+        [PermissionAuthorize((int)EUserRole.ADMIN)]
         public async Task<IActionResult> DeleteSyllabus(long id)
         {
             await _syllabusService.DeleteSyllabusAsync(id);
@@ -55,7 +55,7 @@ namespace AISEA.ApiService.WebApi.Controllers.Syllabus
         /// Creates an assessment for a syllabus (Academic Staff only)
         /// </summary>
         [HttpPost("assessments")]
-        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
         public async Task<IActionResult> CreateAssessment([FromBody] CreateSyllabusAssessmentRequest request)
         {
             var assessmentId = await _syllabusService.CreateAssessmentAsync(request);
@@ -63,10 +63,20 @@ namespace AISEA.ApiService.WebApi.Controllers.Syllabus
         }
 
         /// <summary>
+        /// Creates multiple assessments for syllabi in bulk (Academic Staff only)
+        /// </summary>
+        [HttpPost("assessments/bulk")]
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
+        public async Task<IActionResult> CreateAssessments([FromBody] List<CreateSyllabusAssessmentRequest> requests)
+        {
+            await _syllabusService.CreateSyllabusAssessmentsAsync(requests);
+            return Ok(new { Message = "Assessments created successfully." });
+        }
+        
+        /// <summary>
         /// Creates a learning material for a syllabus (Academic Staff only)
         /// </summary>
         [HttpPost("materials")]
-        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
         public async Task<IActionResult> CreateLearningMaterial([FromBody] CreateSyllabusLearningMaterialRequest request)
         {
             var materialId = await _syllabusService.CreateLearningMaterialAsync(request);
@@ -74,10 +84,20 @@ namespace AISEA.ApiService.WebApi.Controllers.Syllabus
         }
 
         /// <summary>
+        /// Creates multiple learning materials for syllabi in bulk (Academic Staff only)
+        /// </summary>
+        [HttpPost("materials/bulk")]
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
+        public async Task<IActionResult> CreateLearningMaterials([FromBody] List<CreateSyllabusLearningMaterialRequest> requests)
+        {
+            await _syllabusService.CreateSyllabusLearningMaterialsAsync(requests);
+            return Ok(new { Message = "Learning materials created successfully." });
+        }
+        
+        /// <summary>
         /// Creates a learning outcome for a syllabus (Academic Staff only)
         /// </summary>
         [HttpPost("outcomes")]
-        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
         public async Task<IActionResult> CreateLearningOutcome([FromBody] CreateSyllabusLearningOutcomeRequest request)
         {
             var outcomeId = await _syllabusService.CreateLearningOutcomeAsync(request);
@@ -85,10 +105,20 @@ namespace AISEA.ApiService.WebApi.Controllers.Syllabus
         }
 
         /// <summary>
+        /// Creates multiple learning outcomes for syllabi in bulk (Academic Staff only)
+        /// </summary>
+        [HttpPost("outcomes/bulk")]
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
+        public async Task<IActionResult> CreateLearningOutcomes([FromBody] List<CreateSyllabusLearningOutcomeRequest> requests)
+        {
+            await _syllabusService.CreateSyllabusLearningOutcomesAsync(requests);
+            return Ok(new { Message = "Learning outcomes created successfully." });
+        }
+        
+        /// <summary>
         /// Creates a session for a syllabus (Academic Staff only)
         /// </summary>
         [HttpPost("sessions")]
-        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
         public async Task<IActionResult> CreateSession([FromBody] CreateSyllabusSessionRequest request)
         {
             var sessionId = await _syllabusService.CreateSessionAsync(request);
@@ -96,10 +126,20 @@ namespace AISEA.ApiService.WebApi.Controllers.Syllabus
         }
 
         /// <summary>
+        /// Creates multiple sessions for syllabi in bulk (Academic Staff only)
+        /// </summary>
+        [HttpPost("sessions/bulk")]
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
+        public async Task<IActionResult> CreateSessions([FromBody] List<CreateSyllabusSessionRequest> requests)
+        {
+            await _syllabusService.CreateSyllabusSessionsAsync(requests);
+            return Ok(new { Message = "Sessions created successfully." });
+        }
+        
+        /// <summary>
         /// Maps a session to a learning outcome (Academic Staff only)
         /// </summary>
         [HttpPost("sessions/{sessionId}/outcomes/{outcomeId}")]
-        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
         public async Task<IActionResult> MapSessionToOutcome(long sessionId, long outcomeId)
         {
             await _syllabusService.MapSessionToOutcomeAsync(sessionId, outcomeId);