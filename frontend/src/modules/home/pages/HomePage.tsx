import { FeaturesSection } from "../components/sections/FeaturesSection";
import { ProblemSection } from "../components/sections/ProblemSection";
import { ShowcaseSection } from "../components/sections/ShowcaseSection";

export function HomePage() {
  return (
    <div>
      <h1>Home Page</h1>
      <p>Coming soon...</p>
      <ProblemSection/>
      <FeaturesSection/>
      <ShowcaseSection/>
    </div>
  )
}
