@@ .. @@
         /// Creates a new curriculum (Academic Staff only)
         /// </summary>
         [HttpPost]
-        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
         public async Task<IActionResult> CreateCurriculum([FromBody] CreateCurriculumRequest request)
         {
             var curriculumId = await _curriculumService.CreateCurriculumAsync(request);
             return Ok(new { Message = "Curriculum created successfully.", CurriculumId = curriculumId });
         }
+        
+        /// <summary>
+        /// Creates multiple curricula in bulk (Academic Staff only)
+        /// </summary>
+        [HttpPost("bulk")]
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
+        public async Task<IActionResult> CreateCurricula([FromBody] List<CreateCurriculumRequest> requests)
+        {
+            await _curriculumService.CreateCurriculaAsync(requests);
+            return Ok(new { Message = "Curricula created successfully." });
+        }
 
         /// <summary>
         /// Gets paginated list of curricula with optional search and program filter
@@ -33,7 +44,7 @@ namespace AISEA.ApiService.WebApi.Controllers.Curriculum
         /// Updates an existing curriculum (Academic Staff only)
         /// </summary>
         [HttpPut("{id}")]
-        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
         public async Task<IActionResult> UpdateCurriculum(long id, [FromBody] UpdateCurriculumRequest request)
         {
             await _curriculumService.UpdateCurriculumAsync(id, request);
@@ -44,7 +55,7 @@ namespace AISEA.ApiService.WebApi.Controllers.Curriculum
         /// Deletes a curriculum (Admin only) - only if no subjects are assigned
         /// </summary>
         [HttpDelete("{id}")]
-        [PermissionAuthorize(1)] // Admin only
+        [PermissionAuthorize((int)EUserRole.ADMIN)]
         public async Task<IActionResult> DeleteCurriculum(long id)
         {
             await _curriculumService.DeleteCurriculumAsync(id);
@@ -64,7 +75,7 @@ namespace AISEA.ApiService.WebApi.Controllers.Curriculum
         /// Adds a subject to a curriculum (Academic Staff only)
         /// </summary>
         [HttpPost("{id}/subjects")]
-        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
         public async Task<IActionResult> AddSubjectToCurriculum(long id, [FromBody] AddSubjectToCurriculumRequest request)
         {
             await _curriculumService.AddSubjectToCurriculumAsync(id, request);
@@ -75,7 +86,7 @@ namespace AISEA.ApiService.WebApi.Controllers.Curriculum
         /// Removes a subject from a curriculum (Academic Staff only)
         /// </summary>
         [HttpDelete("{id}/subjects/{subjectId}")]
-        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
         public async Task<IActionResult> RemoveSubjectFromCurriculum(long id, long subjectId)
         {
             await _curriculumService.RemoveSubjectFromCurriculumAsync(id, subjectId);