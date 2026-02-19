import { useApiMutation } from '@/shared/hooks/useApiMutation'
import { uploadRoster, type UploadRosterResponse } from '@/services/rosterApi'

export function useUploadRoster() {
  return useApiMutation<UploadRosterResponse, File>({
    mutationFn: (file) => uploadRoster(file),
  })
}
