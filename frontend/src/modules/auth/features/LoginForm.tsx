import { useState } from 'react'
import TextField from '@mui/material/TextField'
import Checkbox from '@mui/material/Checkbox'
import FormControlLabel from '@mui/material/FormControlLabel'
import ArrowForwardIcon from '@mui/icons-material/ArrowForward'
import {
  AuthFormContainer,
  GoogleButton,
  GoogleIcon,
  FormDivider,
  FormDividerText,
  AuthFieldset,
  FormOptions,
  FormLink,
  RememberMeLabel,
  SubmitButton,
} from '../ui'

interface LoginFormProps {
  onSubmit: () => void
  onGoogleLogin: () => void
  onForgotPassword: () => void
  isSubmitting?: boolean
  isGoogleLoading?: boolean
}

export function LoginForm({
  onSubmit,
  onGoogleLogin,
  onForgotPassword,
  isSubmitting = false,
  isGoogleLoading = false,
}: LoginFormProps) {
  const [rememberMe, setRememberMe] = useState(false)
  const isActionDisabled = isSubmitting || isGoogleLoading

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    onSubmit()
  }

  return (
    <AuthFormContainer onSubmit={handleSubmit}>
      <GoogleButton type="button" onClick={onGoogleLogin} disabled={isActionDisabled}>
        <GoogleIcon />
        {isGoogleLoading ? 'Signing in...' : 'Continue with Google'}
      </GoogleButton>

      <FormDivider>
        <FormDividerText>or continue with email</FormDividerText>
      </FormDivider>

      <AuthFieldset aria-label="Email sign in">
        <TextField
          label="Email Address"
          type="email"
          placeholder="you@company.com"
          required
          fullWidth
          autoComplete="email"
        />
        <TextField
          label="Password"
          type="password"
          placeholder="Enter your password"
          required
          fullWidth
          autoComplete="current-password"
        />
      </AuthFieldset>

      <FormOptions>
        <FormControlLabel
          control={
            <Checkbox
              checked={rememberMe}
              onChange={(e) => setRememberMe(e.target.checked)}
              size="small"
            />
          }
          label={<RememberMeLabel>Remember me</RememberMeLabel>}
        />
        <FormLink type="button" onClick={onForgotPassword}>
          Forgot password?
        </FormLink>
      </FormOptions>

      <SubmitButton type="submit" disabled={isActionDisabled}>
        {isSubmitting ? 'Signing in...' : 'Sign In'}
        <ArrowForwardIcon fontSize="small" />
      </SubmitButton>
    </AuthFormContainer>
  )
}
