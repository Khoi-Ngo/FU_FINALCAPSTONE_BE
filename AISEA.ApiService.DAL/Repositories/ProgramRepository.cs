@@ .. @@
                 .OrderBy(p => p.ProgramCode)
                 .ToListAsync();
         }
+        
+        public async Task<(IEnumerable<Program> Programs, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? search = null)
+        {
+            var query = _context.Programs.Where(p => !p.IsDeleted);
+
+            if (!string.IsNullOrEmpty(search))
+            {
+                query = query.Where(p => p.ProgramName.Contains(search) || p.ProgramCode.Contains(search));
+            }
+
+            var totalCount = await query.CountAsync();
+            var programs = await query
+                .OrderBy(p => p.ProgramCode)
+                .Skip((pageNumber - 1) * pageSize)
+                .Take(pageSize)
+                .ToListAsync();
+
+            return (programs, totalCount);
+        }
+        
+        public async Task<bool> HasCurriculaAsync(long programId)
+        {
+            return await _context.Curricula
+                .AnyAsync(c => c.ProgramId == programId && !c.IsDeleted);
+        }
     }
 }