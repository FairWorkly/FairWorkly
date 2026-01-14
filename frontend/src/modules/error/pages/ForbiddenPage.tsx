import { Link } from 'react-router-dom'

export default function ForbiddenPage() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-[#f8fafc]">
      <div className="text-center">
        <h1 className="text-6xl font-bold text-gray-900">403</h1>
        <p className="text-xl text-gray-600 mt-4">Access Denied</p>
        <p className="text-gray-500 mt-2">
          You don't have permission to access this page.
        </p>
        <Link
          to="/fairbot"
          className="mt-6 inline-block px-6 py-3 bg-[#6366f1] text-white rounded-lg"
        >
          Back to FairBot
        </Link>
      </div>
    </div>
  )
}
