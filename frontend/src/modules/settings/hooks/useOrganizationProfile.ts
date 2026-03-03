import { useQueryClient } from '@tanstack/react-query'
import { useApiQuery } from '@/shared/hooks/useApiQuery'
import { useApiMutation } from '@/shared/hooks/useApiMutation'
import {
  settingsApi,
  type OrganizationProfileDto,
  type UpdateOrganizationProfileRequest,
} from '@/services/settingsApi'

const QUERY_KEY = ['settings', 'organization-profile'] as const

export function useOrganizationProfile() {
  return useApiQuery<OrganizationProfileDto>({
    queryKey: QUERY_KEY,
    queryFn: settingsApi.getOrganizationProfile,
  })
}

export function useUpdateOrganizationProfile() {
  const queryClient = useQueryClient()

  return useApiMutation<OrganizationProfileDto, UpdateOrganizationProfileRequest>({
    mutationFn: settingsApi.updateOrganizationProfile,
    onSuccess: (data) => {
      queryClient.setQueryData(QUERY_KEY, data)
    },
  })
}
