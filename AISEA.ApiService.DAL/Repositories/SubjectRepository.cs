@@ .. @@
             return await _context.Subjects
                 .FirstOrDefaultAsync(s => s.SubjectCode == subjectCode && !s.IsDeleted);
         }
+        
+        public async Task<Subject?> GetByIdWithPrerequisitesAsync(long id)
+        {
+            return await _context.Subjects
+                .Include(s => s.Prerequisites)
+                    .ThenInclude(p => p.PrerequisiteSubject)
+                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
+        }
 
         public async Task<(IEnumerable<Subject> Subjects, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? search = null)
         {
@@ .. @@
                 .Where(s => subjectIds.Contains(s.Id) && !s.IsDeleted)
                 .ToListAsync();
         }
+        
+        public async Task<List<Subject>> GetAllActiveAsync()
+        {
+            return await _context.Subjects
+                .Where(s => !s.IsDeleted)
+                .OrderBy(s => s.SubjectCode)
+                .ToListAsync();
+        }
     }
 }