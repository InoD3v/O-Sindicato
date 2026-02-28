import js from '@eslint/js';
import globals from 'globals';
import reactHooks from 'eslint-plugin-react-hooks';
import reactRefresh from 'eslint-plugin-react-refresh';
import tseslint from 'typescript-eslint';

export default tseslint.config(
  { ignores: ['dist', 'node_modules'] },

  // ─── Main config (all TS/TSX files) ──────────────────
  {
    extends: [js.configs.recommended, ...tseslint.configs.recommended],
    files: ['**/*.{ts,tsx}'],
    languageOptions: {
      ecmaVersion: 2020,
      globals: globals.browser,
    },
    plugins: {
      'react-hooks': reactHooks,
      'react-refresh': reactRefresh,
    },
    rules: {
      // ─── React ────────────────────────────────────────
      ...reactHooks.configs.recommended.rules,
      'react-hooks/exhaustive-deps': 'error',
      'react-refresh/only-export-components': [
        'warn',
        { allowConstantExport: true },
      ],

      // ─── TypeScript ──────────────────────────────────
      '@typescript-eslint/no-unused-vars': [
        'warn',
        { argsIgnorePattern: '^_', varsIgnorePattern: '^_' },
      ],
      '@typescript-eslint/no-explicit-any': 'error',
      '@typescript-eslint/consistent-type-imports': [
        'warn',
        { prefer: 'type-imports' },
      ],

      // Naming conventions — see docs/FRONTEND.md § 1
      '@typescript-eslint/naming-convention': [
        'warn',
        // Variables & functions → camelCase (allow PascalCase for components & UPPER_CASE for constants)
        {
          selector: 'variable',
          format: ['camelCase', 'PascalCase', 'UPPER_CASE'],
        },
        {
          selector: 'function',
          format: ['camelCase', 'PascalCase'],
        },
        // Parameters → camelCase (allow _ prefix for unused)
        {
          selector: 'parameter',
          format: ['camelCase'],
          leadingUnderscore: 'allow',
        },
        // Types, Interfaces, Enums → PascalCase (NO "I" prefix — frontend convention)
        {
          selector: 'typeLike',
          format: ['PascalCase'],
        },
        {
          selector: 'interface',
          format: ['PascalCase'],
          custom: { regex: '^I[A-Z]', match: false },
        },
      ],

      // ─── General ─────────────────────────────────────
      'no-console': ['warn', { allow: ['warn', 'error'] }],
      'prefer-const': 'warn',
      'no-var': 'error',
      eqeqeq: ['error', 'always'],
      'no-duplicate-imports': 'error',

      // Prefer "as const" objects over enums — see docs/FRONTEND.md § 8
      'no-restricted-syntax': [
        'error',
        {
          selector: 'TSEnumDeclaration',
          message:
            'Enums are not allowed. Use "as const" objects instead. See docs/FRONTEND.md § 8.',
        },
      ],

      // Max 150 lines per file — see docs/FRONTEND.md § 3
      'max-lines': [
        'warn',
        { max: 150, skipComments: true, skipBlankLines: true },
      ],

      // No TODO/HACK/FIXME left in code — see docs/DEFINITION_OF_DONE.md § 4.2
      'no-warning-comments': [
        'warn',
        { terms: ['todo', 'hack', 'fixme', 'xxx'], location: 'start' },
      ],

      // Prefer @/ alias over deep relative imports — see docs/FRONTEND.md § 2
      'no-restricted-imports': [
        'error',
        {
          patterns: [
            {
              group: ['../../*'],
              message:
                'Avoid deep relative imports (../../). Use the @/ alias instead. See docs/FRONTEND.md § 2.',
            },
          ],
        },
      ],
    },
  },

  // ─── Override: test files (relaxed rules) ─────────────
  {
    files: ['**/*.{test,spec}.{ts,tsx}', '**/test/**/*.{ts,tsx}'],
    rules: {
      'max-lines': 'off',
      'no-console': 'off',
      '@typescript-eslint/no-explicit-any': 'off',
      'no-warning-comments': 'off',
    },
  },
);
