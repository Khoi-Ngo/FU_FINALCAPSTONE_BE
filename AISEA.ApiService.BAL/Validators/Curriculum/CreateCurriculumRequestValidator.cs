@@ .. @@
             RuleFor(x => x.CurriculumCode)
                 .NotEmpty().WithMessage("Curriculum code is required.")
                 .MaxLength(50).WithMessage("Curriculum code must be less than 50 characters.")
-                .Matches(@"^[A-Z0-9_]+$").WithMessage("Curriculum code must contain only uppercase letters, numbers, and underscores.");
+                .Matches(@"^[A-Z0-9_\-]+$").WithMessage("Curriculum code must contain only uppercase letters, numbers, underscores, and hyphens.");
 
             RuleFor(x => x.CurriculumName)
                 .NotEmpty().WithMessage("Curriculum name is required.")