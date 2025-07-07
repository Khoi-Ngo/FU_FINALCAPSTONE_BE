@@ .. @@
             await _comboRepository.UpdateAsync(combo);
         }
 
-        public async Task DeleteComboAsync(long id)
+        public async Task<bool> DeleteComboAsync(long id)
         {
             var combo = await _comboRepository.GetByIdAsync(id);
             if (combo == null || combo.IsDeleted)
@@ .. @@
             combo.DeletedAt = DateTime.UtcNow;
             
             await _comboRepository.UpdateAsync(combo);
+            return true;
         }
 
         public async Task AddSubjectToComboAsync(long comboId, long subjectId)
@@ -107,5 +108,31 @@ namespace AISEA.ApiService.BAL.Services.Combo
 
             await _comboSubjectRepository.RemoveSubjectFromComboAsync(comboId, subjectId);
         }
+        
+        public async Task<bool> CreateCombosAsync(List<CreateComboRequest> requests)
+        {
+            foreach (var request in requests)
+            {
+                // Check if combo name is unique
+                var isNameUnique = await _comboRepository.IsNameUniqueAsync(request.ComboName);
+                if (!isNameUnique)
+                {
+                    throw new InvalidUserCreatedException($"Combo with name '{request.ComboName}' already exists.");
+                }
+
+                // Validate all subjects exist
+                var subjects = await _subjectRepository.GetByIdsAsync(request.SubjectIds);
+                if (subjects.Count != request.SubjectIds.Count)
+                {
+                    throw new NotFoundException("One or more subjects not found.");
+                }
+
+                var combo = _mapper.Map<DAL.Entities.Combo>(request);
+                combo.CreatedAt = DateTime.UtcNow;
+                
+                await _comboRepository.CreateAsync(combo);
+            }
+            return true;
+        }
     }
 }