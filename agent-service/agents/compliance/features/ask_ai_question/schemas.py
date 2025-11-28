from pydantic import BaseModel


class AskAiQuestionRequest(BaseModel):
    question: str


class AskAiQuestionResponse(BaseModel):
    reply: str
