@@ .. @@
         public string? Description { get; set; }
         public DateTime? CreatedAt { get; set; }
         public DateTime? UpdatedAt { get; set; }
+        public List<GetSubjectResponse>? Prerequisites { get; set; }
     }
 }