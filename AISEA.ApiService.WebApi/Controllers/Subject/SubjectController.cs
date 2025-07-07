@@ .. @@
         /// <summary>
         /// Creates a new subject (Academic Staff only)
         /// </summary>
         [HttpPost]
-        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
         public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectRequest request)
         {
             await _subjectService.CreateSubjectAsync(request);
             return Ok(new { Message = "Subject created successfully." });
         }
+        
+        /// <summary>
+        /// Creates multiple subjects in bulk (Academic Staff only)
+        /// </summary>
+        [HttpPost("bulk")]
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
+        public async Task<IActionResult> CreateSubjects([FromBody] List<CreateSubjectRequest> requests)
+        {
+            await _subjectService.CreateSubjectsAsync(requests);
+            return Ok(new { Message = "Subjects created successfully." });
+        }
 
         /// <summary>
         /// Gets paginated list of subjects with optional search
@@ -33,7 +44,7 @@ namespace AISEA.ApiService.WebApi.Controllers.Subject
         /// Updates an existing subject (Academic Staff only)
         /// </summary>
         [HttpPut("{id}")]
-        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
         public async Task<IActionResult> UpdateSubject(long id, [FromBody] UpdateSubjectRequest request)
         {
             await _subjectService.UpdateSubjectAsync(id, request);
@@ -44,7 +55,7 @@ namespace AISEA.ApiService.WebApi.Controllers.Subject
         /// Deletes a subject (Admin only)
         /// </summary>
         [HttpDelete("{id}")]
-        [PermissionAuthorize(1)] // Admin only
+        [PermissionAuthorize((int)EUserRole.ADMIN)]
         public async Task<IActionResult> DeleteSubject(long id)
         {
             await _subjectService.DeleteSubjectAsync(id);
@@ -55,7 +66,7 @@ namespace AISEA.ApiService.WebApi.Controllers.Subject
         /// Adds a prerequisite to a subject (Academic Staff only)
         /// </summary>
         [HttpPost("{id}/prerequisites/{prerequisiteId}")]
-        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
         public async Task<IActionResult> AddPrerequisite(long id, long prerequisiteId)
         {
             await _subjectService.AddPrerequisiteAsync(id, prerequisiteId);
@@ -75,7 +86,7 @@ namespace AISEA.ApiService.WebApi.Controllers.Subject
         /// Removes a prerequisite from a subject (Academic Staff only)
         /// </summary>
         [HttpDelete("{id}/prerequisites/{prerequisiteId}")]
-        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
         public async Task<IActionResult> RemovePrerequisite(long id, long prerequisiteId)
         {
             await _subjectService.RemovePrerequisiteAsync(id, prerequisiteId);