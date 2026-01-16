// For more info, see https://github.com/storybookjs/eslint-plugin-storybook#configuration-flat-config-format
import storybook from 'eslint-plugin-storybook'

import js from '@eslint/js'
import globals from 'globals'
import reactHooks from 'eslint-plugin-react-hooks'
import reactRefresh from 'eslint-plugin-react-refresh'
import tseslint from 'typescript-eslint'
import { defineConfig, globalIgnores } from 'eslint/config'

export default defineConfig([
  globalIgnores(['dist']),

  {
    files: ['**/*.{ts,tsx}'],
    extends: [
      js.configs.recommended,
      tseslint.configs.recommended,
      reactHooks.configs.flat.recommended,
      reactRefresh.configs.vite,
    ],
    languageOptions: {
      ecmaVersion: 2020,
      globals: globals.browser,
    },
    rules: {
      'no-restricted-imports': [
        'error',
        {
          paths: [
            {
              name: '@mui/material/styles',
              importNames: ['styled'],
              message:
                "Do not import 'styled' from '@mui/material/styles'. Use `import { styled } from '@/styles/styled'` instead.",
            },
          ],
        },
      ],
    },
  },

  {
    files: ['src/styles/styled.ts'],
    rules: {
      'no-restricted-imports': 'off',
    },
  },

  {
    files: ['src/**/pages/**/*.{ts,tsx}'],
    rules: {
      'no-restricted-syntax': [
        'error',
        {
          selector: "JSXAttribute[name.name='sx']",
          message:
            'Do not use `sx` in pages. Move styles to feature/ui styled components.',
        },
      ],
    },
  },

  // storybook rules
  // {
  //   files: ['**/*.stories.@(ts|tsx|js|jsx|mjs|cjs)'],
  //   plugins: { storybook },
  //   rules: { ...storybook.configs.recommended.rules },
  // },
])
