@@ .. @@
         /// Creates a new subject combo (Academic Staff only)
         /// </summary>
         [HttpPost]
-        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
         public async Task<IActionResult> CreateCombo([FromBody] CreateComboRequest request)
         {
             var comboId = await _comboService.CreateComboAsync(request);
             return Ok(new { Message = "Combo created successfully.", ComboId = comboId });
         }
+        
+        /// <summary>
+        /// Creates multiple combos in bulk (Academic Staff only)
+        /// </summary>
+        [HttpPost("bulk")]
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
+        public async Task<IActionResult> CreateCombos([FromBody] List<CreateComboRequest> requests)
+        {
+            await _comboService.CreateCombosAsync(requests);
+            return Ok(new { Message = "Combos created successfully." });
+        }
 
         /// <summary>
         /// Gets paginated list of combos with optional search
@@ -33,7 +44,7 @@ namespace AISEA.ApiService.WebApi.Controllers.Combo
         /// Updates an existing combo (Academic Staff only)
         /// </summary>
         [HttpPut("{id}")]
-        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
         public async Task<IActionResult> UpdateCombo(long id, [FromBody] UpdateComboRequest request)
         {
             await _comboService.UpdateComboAsync(id, request);
@@ -44,7 +55,7 @@ namespace AISEA.ApiService.WebApi.Controllers.Combo
         /// Deletes a combo (Admin only)
         /// </summary>
         [HttpDelete("{id}")]
-        [PermissionAuthorize(1)] // Admin only
+        [PermissionAuthorize((int)EUserRole.ADMIN)]
         public async Task<IActionResult> DeleteCombo(long id)
         {
             await _comboService.DeleteComboAsync(id);
@@ -64,7 +75,7 @@ namespace AISEA.ApiService.WebApi.Controllers.Combo
         /// Adds a subject to a combo (Academic Staff only)
         /// </summary>
         [HttpPost("{id}/subjects/{subjectId}")]
-        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
         public async Task<IActionResult> AddSubjectToCombo(long id, long subjectId)
         {
             await _comboService.AddSubjectToComboAsync(id, subjectId);
@@ -75,7 +86,7 @@ namespace AISEA.ApiService.WebApi.Controllers.Combo
         /// Removes a subject from a combo (Academic Staff only)
         /// </summary>
         [HttpDelete("{id}/subjects/{subjectId}")]
-        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
+        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
         public async Task<IActionResult> RemoveSubjectFromCombo(long id, long subjectId)
         {
             await _comboService.RemoveSubjectFromComboAsync(id, subjectId);