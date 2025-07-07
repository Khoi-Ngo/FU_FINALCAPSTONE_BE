@@ .. @@
             RuleFor(x => x.SubjectCode)
                 .NotEmpty().WithMessage("Subject code is required.")
                 .MaxLength(50).WithMessage("Subject code must be less than 50 characters.")
-                .Matches(@"^[A-Za-z0-9]+$").WithMessage("Subject code must contain only letters (uppercase or lowercase) and numbers.");
+                .Matches(@"^[A-Za-z0-9_\-]+$").WithMessage("Subject code must contain only letters, numbers, underscores, and hyphens.");
 
             RuleFor(x => x.SubjectName)
                 .NotEmpty().WithMessage("Subject name is required.")